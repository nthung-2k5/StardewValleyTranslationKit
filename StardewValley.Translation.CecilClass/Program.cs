// See https://aka.ms/new-console-template for more information


using System.Text.Json;
using CsCodeGenerator;
using Mono.Cecil;
using StardewValley.Translation.CecilClass;

var classes = JsonSerializer.Deserialize<Dictionary<string, ClassData>>(File.ReadAllText("test.json"));

using var stardewGameData = AssemblyDefinition.ReadAssembly(Path.Combine(args[0], "StardewValley.GameData.dll"));
var module = stardewGameData.MainModule;

var inheritors = classes.Where(cls => !cls.Value.TopClass)
                                           .ToDictionary(cls => cls.Value.BaseClass, cls => cls.Key);
CsGenerator gen = new() { Path = Path.GetDirectoryName(args[1]), OutputDirectory = Path.GetFileName(args[1]) };
foreach ((string jsonClass, var data) in classes)
{
    var baseType = module.GetType(data.BaseClass);
    var cls = new CsClassGenerator(jsonClass, baseType);

    foreach (string prop in data.Mapping)
    {
        bool isScript = Array.Exists(data.Script, s => s == prop);
        var field = baseType.Fields.First(t => t.Name == prop);
        string type = field.FieldType.FullName.Contains("System.Collections.Generic.List")
                    ? ((GenericInstanceType)field.FieldType).GenericArguments[0].FullName
                    : field.FieldType.FullName;
        inheritors.TryGetValue(type, out string newType);
        cls.Property(field, isScript, newType);
    }
    
    cls.Finish(gen);
}

File.WriteAllText(Path.Combine(args[1], "ClassTranslation.g.cs"), SourceCodeHelper.ClassTranslation(classes.Where(cls => cls.Value.TopClass).Select(cls => (cls.Key, cls.Value.BaseClass))));
File.WriteAllText(Path.Combine(args[1], "JsonSourceGenerationContext.cs"), SourceCodeHelper.JsonSourceGenerator(classes.Select(cls => cls.Key).Concat(classes.Select(cls => cls.Value.BaseClass))));

gen.CreateFiles();
