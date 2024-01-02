using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace StardewValley.Translation.JsonClass;

public interface IJsonClass
{
    JsonNode Apply(JsonNode node);
    JsonNode JsonContent { set; }
}

public abstract class BaseJsonClass<T> : IJsonClass where T : class
{
    protected abstract void Read(T data);
    private static JsonTypeInfo<T> Context => (JsonTypeInfo<T>)JsonSourceGenerationContext.Default.GetTypeInfo(typeof(T))!;
    public JsonNode JsonContent { set => Content = value.Deserialize(Context)!; }
    public T Content { set => Read(value); }
    public JsonNode Apply(JsonNode node)
    {
        var deserialized = node.Deserialize(Context)!;
        Apply(deserialized);
        return JsonSerializer.SerializeToNode(deserialized, Context)!;
    }

    public abstract void Apply(T data);
}