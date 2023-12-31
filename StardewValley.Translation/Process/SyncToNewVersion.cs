using System.Text.Json.Nodes;
using Json.Patch;
using StardewValley.Translation.Formats;
using StardewValley.Translation.Helper;
using static StardewValley.Translation.Helper.JsonFileHelper;

namespace StardewValley.Translation.Process;

public record SyncContext(JsonNode OldContent, JsonNode Content) : ProcessContext(Content);

public class SyncToNewVersion(string oldFolder, string newFolder) : BaseProcess<SyncContext>(newFolder)
{
    protected override void OnFileProcessed((string file, JsonNode content) context)
    {
        base.OnFileProcessed(context);
        var exporter = new JsonFormat { Content = context.content.Parent };
        exporter.Export(context.file);
    }
    protected override (SyncContext, JsonNode?) ProcessFile(string file)
    {
        GetLanguageData(oldFolder, file, Language).TryGetPropertyValue("content", out var oldContent);
        
        var newJson = GetLanguageData(Folder, file, Language);
        var newContent = newJson["content"]!;

        oldContent ??= newContent.StubContainer();

        return (new SyncContext(oldContent, newContent), newJson["readers"]);
    }
    protected override JsonNode ProcessPrimitive(SyncContext context)
    {
        var (oldContent, newContent) = context;
        var patch = newContent.CreatePatch(oldContent);
        var syncPatch = new JsonPatch(patch.Operations.Where(op => op.Op == OperationType.Replace));

        context.Content.ReplaceWith(syncPatch.Apply(newContent).Result!);

        return context.Content;
    }
    protected override JsonNode ProcessClass(SyncContext context, string type)
    {
#if SV_1_6
#endif
        throw new NotImplementedException();
    }
}
