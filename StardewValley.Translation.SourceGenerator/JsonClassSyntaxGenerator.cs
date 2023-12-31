using System.CodeDom.Compiler;
using System.Collections.Immutable;
using CsCodeGenerator;
using CsCodeGenerator.Enums;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
namespace StardewValley.Translation.SourceGenerator;

public class JsonClassSyntaxGenerator
{
    private const string BaseClassParameter = "content";
    private FileModel model;
    private ClassModel ClassModel => model.Classes[0];
    private List<IFieldSymbol> fieldSymbols = null!;
    private string baseClass;
    public JsonClassSyntaxGenerator(string @namespace, string className, string baseClass)
    {
        model = new FileModel(className)
        {
            Namespace = @namespace,
            Classes = [new ClassModel(className)]
        };
        this.baseClass = baseClass;
        ClassModel.BaseClass = $"BaseJsonClass<{baseClass}>";
    }

    public void Properties(IEnumerable<IFieldSymbol> fields)
    {
        fieldSymbols = fields.ToList();
        ClassModel.Properties = fieldSymbols.ConvertAll(FieldToProperty);
    }

    private static Property FieldToProperty(IFieldSymbol field)
    {
        string type = field.Type.ToDisplayString();
        if (GetOptionalAttribute(field.GetAttributes(), out var attribute))
        {
            type = type.TrimEnd('?') + '?';
        }
        return new Property(type, field.Name)
        {
            Attributes = attribute
        };
    }

    private void AutoConstructor()
    {
        ClassModel.Constructors =
        [
            new Constructor(ClassModel.Name)
            {
                Parameters = [new Parameter(baseClass, BaseClassParameter)],
                BodyLines = fieldSymbols.ConvertAll(AssignProperty)
            }
        ];
    }

    private void ApplyMethod()
    {
        ClassModel.Methods =
        [
            new Method(BuiltInDataType.Void, "Apply")
            {
                Parameters = [new Parameter("ref " + baseClass, BaseClassParameter)],
                BodyLines = fieldSymbols.ConvertAll(InverseAssignProperty)
            }
        ];
    }
    private static string AssignProperty(ISymbol field) => $"{field.Name} = {BaseClassParameter}.{field.Name}";
    private static string InverseAssignProperty(ISymbol field) => $"{field.Name} = {BaseClassParameter}.{field.Name}";

    private static bool GetOptionalAttribute(ImmutableArray<AttributeData> attr, out List<AttributeModel>? attributeList)
    {
        if ((from attribute in attr
             where attribute.AttributeClass!.ToDisplayString() == "Microsoft.Xna.Framework.Content.ContentSerializer"
             select attribute.NamedArguments.FirstOrDefault(arg => arg.Key == "Optional"))
            .Any(optionalArg => !optionalArg.Equals(default) && (bool)optionalArg.Value.Value!))
        {
            attributeList = [
                new AttributeModel("JsonIgnore")
                    { SingleParameter = new Parameter("Condition = JsonIgnoreCondition.WhenWritingDefault") }
            ];
            return true;
        }

        attributeList = null;
        return false;
    }
}