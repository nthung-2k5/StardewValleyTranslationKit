namespace StardewValley.Translation.CecilClass;
internal static class SourceCodeHelper
{
    public static string ClassTranslation(IEnumerable<(string Type, string BaseType)> types) =>
$$"""
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
namespace StardewValley.Translation.JsonClass;

public static partial class ClassTranslation
{
    private static JsonSerializerOptions IgnoreNullSerializerOptions = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
    public static JsonNode Convert(JsonNode node, string type)
    {
        return type switch
        {
            {{string.Join(NewLine(',', 3), types.Select(type => $"\"{type.BaseType}\" => Convert<{type.Type}>(node)"))}},
            _ => throw new ArgumentException(nameof(type))
        };
    }
}
""";

    public static string JsonSourceGenerator(IEnumerable<string> types) =>
$$"""
using StardewValley.GameData.Movies;
using System.Text.Json.Serialization;

namespace StardewValley.Translation.JsonClass;

[JsonSourceGenerationOptions(IncludeFields = true)]
{{string.Join(NewLine(' ', 0), types.Select(type => $"[JsonSerializable(typeof({type.Split('.')[^1]}))]"))}}
public partial class JsonSourceGenerationContext: JsonSerializerContext
{
    
}
""";

    private static string NewLine(char newline, int indent) => new string([newline, '\n']) + new string(' ', indent * 4);
}
