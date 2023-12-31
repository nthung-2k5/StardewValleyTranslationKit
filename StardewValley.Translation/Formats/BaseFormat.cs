using System.Text.Json.Nodes;
using MrMeeseeks.Visitor;

namespace StardewValley.Translation.Formats;

public partial interface IFormat
{
    void Export(string file);
    void Import(string file);
    JsonNode? Content { get; set; }
}

public partial interface ICsvFormat : IFormat
{
    List<TranslationLine> Lines { get; }
}
public partial interface IJsonFormat: IFormat {}
[VisitorInterface(typeof(IFormat))]
public partial interface IFormatVisitor {}
public abstract class BaseFormat
{
    public void Export(string file)
    {
        using var stream = File.Create(file);
        Export(stream);
    }

    protected abstract void Export(FileStream stream);

    public void Import(string file)
    {
        using var stream = File.OpenRead(file);
        Import(stream);
    }
    
    protected abstract void Import(FileStream stream);

    public JsonNode? Content { get; set; }
}