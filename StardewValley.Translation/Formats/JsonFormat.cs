using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace StardewValley.Translation.Formats;
public partial class JsonFormat: BaseFormat, IJsonFormat
{
    protected override void Export(FileStream stream)
    {
        using Utf8JsonWriter writer = new(stream, new JsonWriterOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, Indented = true });
        Content!.WriteTo(writer);
    }

    protected override void Import(FileStream stream) => Content = JsonNode.Parse(stream);
}
