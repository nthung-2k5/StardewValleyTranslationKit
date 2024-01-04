// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using CsCodeGenerator;
using Mono.Cecil;
using StardewValley.Translation.CecilClass;

var classes = JsonSerializer.Deserialize(File.ReadAllText("class.json"), JsonSourceGenerationContext.Default.DictionaryStringClassData);

using AssemblyDefinition stardewGameData = AssemblyDefinition.ReadAssembly(Path.Combine(args[0], "StardewValley.GameData.dll"));
ModuleDefinition module = stardewGameData.MainModule;

var inheritors = classes.ToDictionary(cls => cls.Value.BaseClass, cls => cls.Key);

CsGenerator gen = new() { Path = Path.GetDirectoryName(args[1]), OutputDirectory = Path.GetFileName(args[1]) };

foreach ((string jsonClass, ClassData data) in classes)
{
    TypeDefinition baseType = module.GetType(data.BaseClass);
    var cls = new CsClassGenerator(jsonClass, baseType);

    foreach (string prop in data.Mapping)
    {
        bool isScript = Array.Exists(data.Script, s => s == prop);
        FieldDefinition field = baseType.Fields.First(t => t.Name == prop);
        
        string type = field.FieldType.FullName.Contains("System.Collections.Generic.List")
            ? ((GenericInstanceType)field.FieldType).GenericArguments[0].FullName
            : field.FieldType.FullName;
        
        inheritors.TryGetValue(type, out string newType);
        cls.Property(field, isScript, newType);
    }

    cls.Finish(gen);
}

var topClasses = classes.Where(static cls => cls.Value.TopClass).Select(static kv => (kv.Key, kv.Value.BaseClass)).ToList();
File.WriteAllText(Path.Combine(args[1], "ClassTranslation.g.cs"), SourceCodeHelper.ClassTranslation(topClasses));

var allTypes = classes.SelectMany(static cls => new[] { cls.Key, cls.Value.BaseClass }).ToList();
allTypes.Sort();

File.WriteAllText(Path.Combine(args[1], "JsonSourceGenerationContext.g.cs"), SourceCodeHelper.JsonSourceGenerator(allTypes));

gen.CreateFiles();
