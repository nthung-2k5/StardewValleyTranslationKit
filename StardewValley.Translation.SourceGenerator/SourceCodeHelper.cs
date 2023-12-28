using System;
using System.Collections.Generic;
using System.Text;

namespace StardewValley.Translation.SourceGenerator;
internal static class SourceCodeHelper
{
    public static string ClassTranslation(IEnumerable<(string Type, string BaseType)> types) =>
$$"""
using System.Text.Json;
using System.Text.Json.Nodes;
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

    public static JsonNode Convert<T>(JsonNode node) where T: BaseJsonClass, new()
    {
        return node.GetValueKind() switch
        {
            JsonValueKind.Object => ConvertObject<T>(node.AsObject()),
            JsonValueKind.Array => ConvertArray<T>(node.AsArray()),
            _ => throw new NotImplementedException()
        };
    }

    private static JsonArray ConvertArray<T>(JsonArray array) where T: BaseJsonClass, new()
    {
        var converted = new JsonArray();
        foreach (var item in array)
        {
            var cls = new T();
            cls.Read(item);

            converted.Add(JsonSerializer.SerializeToNode(cls));
        }
        return converted;
    }

    private static JsonObject ConvertObject<T>(JsonObject obj) where T: BaseJsonClass, new()
    {
        var converted = new JsonObject();
        foreach (var (k, v) in obj)
        {
            var cls = new T();
            cls.Read(v);

            converted.Add(k, JsonSerializer.SerializeToNode(cls));
        }
        return converted;
    }
}
""";

    public static string JsonPartialClassCode(AttributeItem item)
    {
        return Extensions.GetResource(item.ClassSyntax, parent => $$"""partial {{parent.Keyword}} {{parent.Name}} {{parent.Constraints}}: StardewValley.Translation.JsonClass.BaseJsonClass<{{item.BaseType}}> { }""");
    }

    private static string NewLine(char newline, int indent) => newline + '\n' + new string(' ', indent * 4);
}
