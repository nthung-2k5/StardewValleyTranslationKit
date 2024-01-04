using System.Text.Json.Nodes;
using MrMeeseeks.Visitor;

namespace StardewValley.Translation.Formats;

public partial interface IFormat
{
    JsonObject? Content { get; set; }
    void Export(string file);
    void Import(string file);
}

public interface ICsvFormat : IFormat
{
    List<TranslationLine> Lines { get; }
}

public interface IJsonFormat : IFormat
{
}

[VisitorInterface(typeof(IFormat))]
public partial interface IFormatVisitor
{
}

public abstract class BaseFormat
{
    public JsonObject Content { get; set; } = new();

    public void Export(string file)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(file)!);
        using FileStream stream = File.Create(file);
        Export(stream);
    }

    protected abstract void Export(FileStream stream);

    public void Import(string file)
    {
        using FileStream stream = File.OpenRead(file);
        Import(stream);
    }

    protected abstract void Import(FileStream stream);
}
