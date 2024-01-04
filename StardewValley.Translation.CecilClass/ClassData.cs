using System.Text.Json.Serialization;

namespace StardewValley.Translation.CecilClass;

public record ClassData(string BaseClass, string[] Mapping, string[] Script, bool TopClass);

[JsonSerializable(typeof(ClassData))]
[JsonSerializable(typeof(Dictionary<string, ClassData>))]
public partial class JsonSourceGenerationContext: JsonSerializerContext;