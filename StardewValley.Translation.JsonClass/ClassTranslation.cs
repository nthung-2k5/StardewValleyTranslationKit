using System.Text.Json;
using System.Text.Json.Nodes;
namespace StardewValley.Translation.JsonClass;

public static partial class ClassTranslation
{
    private static JsonNode Convert<T>(JsonNode node) where T: IJsonClass, new()
    {
        return node.GetValueKind() switch
        {
            JsonValueKind.Object => ConvertObject<T>(node.AsObject()),
            JsonValueKind.Array => ConvertArray<T>(node.AsArray()),
            _ => throw new ArgumentOutOfRangeException(node.GetValueKind() + "is not a JSON container")
        };
    }

    private static JsonArray ConvertArray<T>(JsonArray array) where T: IJsonClass, new()
    {
        var converted = new JsonArray();
        foreach (JsonNode? item in array)
        {
            var cls = new T { JsonContent = item! };
            converted.Add(JsonSerializer.SerializeToNode(cls, typeof(T), JsonSourceGenerationContext.Default));
        }
        return converted;
    }

    private static JsonObject ConvertObject<T>(JsonObject obj) where T: IJsonClass, new()
    {
        var converted = new JsonObject();
        foreach ((string k, JsonNode? v) in obj)
        {
            var cls = new T { JsonContent = v! };
            converted.Add(k, JsonSerializer.SerializeToNode(cls, typeof(T), JsonSourceGenerationContext.Default));
        }
        return converted;
    }
    
    private static void Apply<T>(JsonNode node, JsonNode translation) where T: IJsonClass, new()
    {
        switch (node.GetValueKind())
        {
            case JsonValueKind.Object:
                ApplyObject<T>(node.AsObject(), translation);
                break;
            case JsonValueKind.Array:
                ApplyArray<T>(node.AsArray(), translation);
                break;
            case JsonValueKind.Undefined:
            case JsonValueKind.String:
            case JsonValueKind.Number:
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
            default:
                throw new ArgumentOutOfRangeException(node.GetValueKind() + "is not a JSON container");
        }
    }

    private static void ApplyArray<T>(JsonArray array, JsonNode translation) where T: IJsonClass, new()
    {
        for (int i = 0; i < array.Count; i++)
        {
            var cls = (T)translation[i].Deserialize(typeof(T), JsonSourceGenerationContext.Default)!;
            array[i]!.ReplaceWith(cls.Apply(array[i]!));
        }
    }

    private static void ApplyObject<T>(JsonObject obj, JsonNode translation) where T: IJsonClass, new()
    {
        foreach ((string k, JsonNode? v) in obj)
        {
            var cls = (T)translation[k].Deserialize(typeof(T), JsonSourceGenerationContext.Default)!;
            obj[k]!.ReplaceWith(cls.Apply(v!));
        }
    }
}