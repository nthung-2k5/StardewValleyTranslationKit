using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
namespace StardewValley.Translation.JsonClass;

public static partial class ClassTranslation
{
    private static JsonNode Convert<T>(JsonNode node) where T: IJsonClass, new()
    {
        return node.GetValueKind() switch
        {
            JsonValueKind.Object => ConvertObject<T>(node.AsObject()),
            JsonValueKind.Array => ConvertArray<T>(node.AsArray()),
            _ => throw new NotImplementedException()
        };
    }

    private static JsonArray ConvertArray<T>(JsonArray array) where T: IJsonClass, new()
    {
        var converted = new JsonArray();
        foreach (var item in array)
        {
            var cls = new T { JsonContent = item! };
            converted.Add(JsonSerializer.SerializeToNode(cls, typeof(T), JsonSourceGenerationContext.Default));
        }
        return converted;
    }

    private static JsonObject ConvertObject<T>(JsonObject obj) where T: IJsonClass, new()
    {
        var converted = new JsonObject();
        foreach ((string k, var v) in obj)
        {
            var cls = new T { JsonContent = v! };
            converted.Add(k, JsonSerializer.SerializeToNode(cls, typeof(T), JsonSourceGenerationContext.Default));
        }
        return converted;
    }
}