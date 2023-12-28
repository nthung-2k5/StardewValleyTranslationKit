using StardewValley.Formats;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SVTranslation.Formats;
public class JsonFormat(JsonNode? content = null, FormatMode? mode = null) : BaseFormat(content, mode)
{
    public override void Export(FileStream stream)
    {
        using Utf8JsonWriter writer = new(stream, new() { Indented = true });
        Content!.WriteTo(writer);
    }

    public override void Import(FileStream stream) => content = JsonNode.Parse(stream);
}
