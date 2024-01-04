using System.CodeDom.Compiler;
using System.Text;
using System.Text.Json.Serialization;
using CsCodeGenerator;
using CsCodeGenerator.Enums;
using Mono.Cecil;

namespace StardewValley.Translation.CecilClass;

[Flags]
public enum Type
{
    String = 1,
    Script = 2,
    List = 4,
    Custom = 8
}

public class CsClassGenerator(string className, TypeDefinition baseType)
{
    private const string Namespace = "StardewValley.Translation.JsonClass";
    private const string BaseParam = "content";

    private readonly ClassModel jsonClass = new(className)
    {
        BaseClass = $"BaseJsonClass<{baseType.Name}>",
        AccessModifier = AccessModifier.Internal
    };

    private readonly List<(Property, Type)> properties = [];

    public void Property(FieldDefinition field, bool script, string replaceType)
    {
        Type type = GetFieldType(field, script);
        bool optional = IsOptional(field.CustomAttributes);

        StringBuilder typename = GetTypeName(field, type, replaceType);

        AttributeModel jsonIgnore = null;

        if (optional)
        {
            typename.Append('?');
            jsonIgnore = new AttributeModel("JsonIgnore")
            {
                SingleParameter = new Parameter($"{nameof(JsonIgnoreAttribute.Condition)} = {nameof(JsonIgnoreCondition)}.{nameof(JsonIgnoreCondition.WhenWritingNull)}")
            };
        }

        var prop = new Property(typename.ToString(), field.Name);

        if (optional)
        {
            prop.AddAttribute(jsonIgnore);
        }

        properties.Add((prop, type));
    }

    public void Finish(CsGenerator gen)
    {
        jsonClass.Properties.AddRange(properties.Select(prop => prop.Item1));
        GenerateRead();
        GenerateInverseApply();

        gen.Files.Add(new FileModel(jsonClass.Name)
        {
            Namespace = Namespace,
            UsingDirectives =
            [
                "System;", "System.Collections.Generic;", "StardewValley.GameData.Movies;",
                "System.Text.Json.Serialization;", "System.Text.Json.Serialization.Metadata;"
            ],
            Classes = [jsonClass]
        });
    }

    private void GenerateRead()
    {
        var ctor = new Method(BuiltInDataType.Void, "Read")
        {
            AccessModifier = AccessModifier.Protected,
            SingleKeyWord = KeyWord.Override,
            Parameters = [new Parameter(baseType.Name, BaseParam)],
            BodyLines = properties.Select(prop => CreateStatement(prop.Item1, prop.Item2, false))
                                  .SelectMany(s => s).ToList()
        };
        jsonClass.Methods.Add(ctor);
    }

    private void GenerateInverseApply()
    {
        var apply = new Method(BuiltInDataType.Void, "Apply")
        {
            SingleKeyWord = KeyWord.Override,
            Parameters = [new Parameter(baseType.Name, BaseParam)],
            BodyLines = properties.Select(prop => CreateStatement(prop.Item1, prop.Item2, true))
                                  .SelectMany(s => s).ToList()
        };
        jsonClass.Methods.Add(apply);
    }

    private static string[] CreateStatement(Property property, Type type, bool inverse)
    {
        StringWriter innerWriter = new();
        IndentedTextWriter writer = new(innerWriter);

        StatementCreator creator;

        if (type.HasFlag(Type.Script))
        {
            creator = ScriptAssignmentSyntax;
        }
        else if (type.HasFlag(Type.List))
        {
            creator = type.HasFlag(Type.Custom) ? ListAssignmentSyntax : SimpleAssignmentSyntax;
        }
        else if (type.HasFlag(Type.Custom))
        {
            creator = SimpleApplySyntax;
        }
        else
        {
            creator = SimpleAssignmentSyntax;
        }

        creator(property, $"{BaseParam}.{property.Name}", inverse, writer);

        return innerWriter.ToString().Split(writer.NewLine, StringSplitOptions.RemoveEmptyEntries);
    }

    private static void SimpleAssignmentSyntax(Property property, string baseProperty, bool inverse, IndentedTextWriter writer)
    {
        (string lhs, string rhs) = (property.Name, baseProperty);

        if (inverse)
        {
            (lhs, rhs) = (rhs, lhs);
        }

        writer.WriteLine($"{lhs} = {rhs};");
    }

    private static void SimpleApplySyntax(Property property, string baseProperty, bool inverse,
                                          IndentedTextWriter writer)
    {
        // if (data.SpecialResponses is not null)
        // {
        //     SpecialResponses = new JsonSpecialResponses { Content = data.SpecialResponses };
        // }

        // SpecialResponses?.Apply(data.SpecialResponses);

        bool nullable = property.CustomDataType.Contains('?');

        if (!inverse)
        {
            string assign = $"{property.Name} = new {(nullable ? property.CustomDataType[..^1] : property.CustomDataType)} {{ Content = {baseProperty} }};";

            if (nullable)
            {
                writer.WriteLine($"if ({baseProperty} is not null)");
                writer.WriteLine('{');
                writer.Indent++;
                writer.WriteLine(assign);
                writer.Indent--;
                writer.WriteLine('}');
            }
            else
            {
                writer.WriteLine(assign);
            }
        }
        else
        {
            InvocationBasedOnNullable(writer, nullable, property.Name, "Apply", baseProperty);
        }
    }

    private static void ListAssignmentSyntax(Property property, string baseProperty, bool inverse,
                                             IndentedTextWriter writer)
    {
        // Scenes = data.Scenes.ConvertAll(scene => new JsonMovieScene { Content = scene });

        // int i = 0;
        // data.Scenes.ForEach(scene => Scenes[i++].Apply(scene));
        bool nullable = property.CustomDataType.Contains('?');

        string type = property.CustomDataType["List<".Length..property.CustomDataType.LastIndexOf('>')];
        string lambdaArg = property.Name[..^1].ToLower();

        if (!inverse)
        {
            writer.Write($"{property.Name} = ");
            InvocationBasedOnNullable(writer, nullable, baseProperty, nameof(List<object>.ConvertAll), $"{lambdaArg} => new {type} {{ Content = {lambdaArg} }}");
        }
        else
        {
            writer.WriteLine("int i = 0;");
            InvocationBasedOnNullable(writer, nullable, baseProperty, nameof(List<object>.ForEach), $"{lambdaArg} => {property.Name}[i++].Apply({lambdaArg})");
        }
    }

    private static void ScriptAssignmentSyntax(Property property, string baseProperty, bool inverse, IndentedTextWriter writer)
    {
        // Script = Script.From(data.Script);

        // Script?.Apply(ref data.Script);

        bool nullable = property.CustomDataType.Contains('?');

        if (!inverse)
        {
            writer.WriteLine($"{property.Name} = Script.From({baseProperty});");
        }
        else
        {
            InvocationBasedOnNullable(writer, nullable, property.Name, "Apply", $"ref {baseProperty}");
        }
    }

    private static void InvocationBasedOnNullable(IndentedTextWriter writer, bool nullable, string property, string method, params string[] args)
    {
        writer.Write(property);

        if (nullable)
        {
            writer.Write('?');
        }

        writer.Write($".{method}({string.Join(", ", args)});");
    }

    private static bool IsOptional(IEnumerable<CustomAttribute> attr)
    {
        CustomAttribute first = attr.FirstOrDefault(attribute => attribute.AttributeType.FullName == "Microsoft.Xna.Framework.Content.ContentSerializerAttribute" &&
                                                                 attribute.Properties.Any(arg => arg.Name == "Optional" && (bool)arg.Argument.Value));

        return first is not null;
    }

    private static Type GetFieldType(FieldReference field, bool script)
    {
        string type = field.FieldType.FullName;
        Type res = 0;

        if (script)
        {
            res |= Type.Script;
        }
        else if (type.Contains("System.String"))
        {
            res |= Type.String;
        }
        else
        {
            res = Type.Custom;
        }

        if (type.Contains("System.Collections.Generic.List"))
        {
            res |= Type.List;
        }

        return res;
    }

    private static StringBuilder GetTypeName(FieldReference field, Type type, string replaceType)
    {
        StringBuilder typename = new();

        if (type.HasFlag(Type.List))
        {
            typename.Append("List<");
        }

        if (type.HasFlag(Type.String))
        {
            typename.Append("string");
        }
        else if (type.HasFlag(Type.Script))
        {
            typename.Append("Script");
        }
        else
        {
            typename.Append(replaceType ?? (type.HasFlag(Type.List) ? ((GenericInstanceType)field.FieldType).GenericArguments[0].Name : field.FieldType.Name));
        }

        if (type.HasFlag(Type.List))
        {
            typename.Append('>');
        }

        return typename;
    }

    private delegate void StatementCreator(Property property, string baseProperty, bool inverse, IndentedTextWriter writer);
}
