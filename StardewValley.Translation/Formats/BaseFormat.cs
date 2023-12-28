using System.Text.Json.Nodes;

namespace StardewValley.Formats;
public abstract class BaseFormat
{
    protected BaseFormat(JsonNode? content = null, FormatMode? mode = null)
    {
        Mode = mode ?? (content == null ? FormatMode.Import : FormatMode.Export);
        this.content = content;
    }
    public FormatMode Mode { get; set; }
    public void Export(string file)
    {
        using FileStream stream = File.Create(file);
        Export(stream);
    }
    public abstract void Export(FileStream stream);

    public void Import(string file)
    {
        using FileStream stream = File.Create(file);
        Import(stream);
    }
    public abstract void Import(FileStream stream);

    public JsonNode? Content
    {
        get => content;
        set
        {
            if (Mode == FormatMode.Import)
            {
                throw new InvalidOperationException($"Disallowed mode: {Mode}");
            }

            content = value;
        }
    }
    protected JsonNode? content;
}

[Flags]
public enum FormatMode
{
    Import = 1,
    Export = 2,
    All = Import | Export
}
