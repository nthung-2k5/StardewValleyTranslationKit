namespace StardewValley.Translation.CecilClass;

internal static class SourceCodeHelper
{
    public static string ClassTranslation(List<(string Type, string BaseType)> types) =>
$$"""
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
namespace StardewValley.Translation.JsonClass;

public static partial class ClassTranslation
{
    public static JsonNode Convert(JsonNode node, string type)
    {
        return type switch
        {
            {{string.Join(NewLine(',', 3), types.Select(type => $"\"{type.BaseType}\" => Convert<{type.Type}>(node)"))}},
            _ => throw new ArgumentException(nameof(type))
        };
    }
    
    public static void Apply(JsonNode node, JsonNode translation, string type)
    {
        switch (type)
        {
            {{string.Join(NewLine(';', 3), types.Select(type => $"case \"{type.BaseType}\":" + NewLine(4) + $"Apply<{type.Type}>(node, translation)" + NewLine(';', 4) + "break"))}};
            default:
                throw new ArgumentException(nameof(type));
        }
    }
}
""";
    public static string JsonSourceGenerator(IEnumerable<string> types) =>
$"""
using StardewValley.GameData.Movies;
using System.Text.Json.Serialization;

namespace StardewValley.Translation.JsonClass;

[JsonSourceGenerationOptions(IncludeFields = true)]
{string.Join('\n', types.Select(type => $"[JsonSerializable(typeof({type.Split('.')[^1]}))]"))}
public partial class JsonSourceGenerationContext: JsonSerializerContext;
""";

    private static string NewLine(char newline, int indent) => newline + NewLine(indent);
    private static string NewLine(int indent) => '\n' + new string(' ', indent * 4);
}
