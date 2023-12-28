using System.Text.Json;
using System.Text.Json.Nodes;
namespace StardewValley.Translation.JsonClass;

public abstract class BaseJsonClass
{
    public abstract void Read(JsonNode node);
    public abstract JsonNode Write(JsonNode node);
}

public abstract class BaseJsonClass<T> : BaseJsonClass where T : class
{
    public override void Read(JsonNode node) => Read(node.GetValue<T>());
    public abstract void Read(T data);
    public override JsonNode Write(JsonNode node)
    {
        var deserialized = node.GetValue<T>();
        Write(ref deserialized);
        return JsonSerializer.SerializeToNode(deserialized)!;
    }
    public abstract void Write(ref T data);
}