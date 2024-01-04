using System.Text.Json.Nodes;
using Json.Patch;
using StardewValley.Translation.Formats;
using StardewValley.Translation.Helper;
using StardewValley.Translation.JsonClass;

namespace StardewValley.Translation.Process;

public record UpdateContext(JsonNode Content, JsonNode Translation): ProcessContext(Content);
public class StardewUpdate(string folder, string transPath, IFormat format, string? exportPath = null): BaseProcess<UpdateContext>(folder, exportPath), IFormatVisitor
{
    private JsonNode Translation => format.Content!;
    public override void Process()
    {
        format.Accept(this);
        base.Process();
    }
    
    protected override (UpdateContext, JsonNode?) ProcessFile(string file)
    {
        JsonObject json = JsonFileHelper.GetLanguageData(Folder, file, Language);
        JsonNode content = json["content"]!;

        return (new UpdateContext(content, Translation[file]!), json["readers"]);
    }

    protected override JsonNode ProcessPrimitive(UpdateContext context)
    {
        var patch = context.Content.CreatePatch(context.Translation).Operations.Where(op => op.Op == OperationType.Replace);
        var truePatch = new JsonPatch(patch);
        context.Content.ReplaceWith(truePatch.Apply(context.Content).Result!);

        return context.Content;
    }

    protected override JsonNode ProcessClass(UpdateContext context, string type)
    {
        ClassTranslation.Apply(context.Content, context.Translation, type);

        return context.Content;
    }

    protected override void OnFileProcessed((string file, JsonNode content) e)
    {
        base.OnFileProcessed(e);
        var exporter = new JsonFormat { Content = e.content.Parent!.AsObject() };
        
        string file = Path.Combine(ExportFolder, $"{e.file}.{Language}.json");
        Directory.CreateDirectory(Path.GetDirectoryName(file)!);
        exporter.Export(file);
    }

    public void VisitIJsonFormat(IJsonFormat element)
    {
        element.Import(transPath + "content.new.json");
    }

    public void VisitICsvFormat(ICsvFormat element)
    {
        foreach (string csv in Directory.EnumerateFiles(transPath, "*.csv"))
        {
            element.Import(csv);
        }
    }
}
