using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace StardewValley.Translation.JsonClass;

public abstract class BaseJsonClass
{
    public abstract void Read(JsonNode node);
    public abstract JsonNode Write(JsonNode node);
    protected static readonly JsonSerializerOptions WithFieldSerializerOptions = new() { IncludeFields = true };
}

public abstract class BaseJsonClass<T> : BaseJsonClass where T : class
{
    public override void Read(JsonNode node) => Read(node.Deserialize<T>(WithFieldSerializerOptions)!);
    public abstract void Read(T data);
    public override JsonNode Write(JsonNode node)
    {
        var deserialized = node.Deserialize<T>(WithFieldSerializerOptions)!;
        Write(ref deserialized);
        return JsonSerializer.SerializeToNode(deserialized, WithFieldSerializerOptions)!;
    }
    public abstract void Write(ref T data);
}