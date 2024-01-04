using System.Text.Json.Nodes;
using Json.Patch;
using StardewValley.Translation.Formats;
using StardewValley.Translation.Helper;
using static StardewValley.Translation.Helper.JsonFileHelper;

namespace StardewValley.Translation.Process;

public record SyncContext(JsonNode OldContent, JsonNode Content) : ProcessContext(Content);

public class SyncToNewVersion(string oldFolder, string newFolder, string? exportFolder = null) : BaseProcess<SyncContext>(newFolder, exportFolder)
{
    protected override void OnFileProcessed((string file, JsonNode content) context)
    {
        base.OnFileProcessed(context);
        var exporter = new JsonFormat { Content = context.content.Parent!.AsObject() };
        exporter.Export(Path.Combine(ExportFolder, Path.GetRelativePath(Folder, context.file)));
    }

    protected override (SyncContext, JsonNode?) ProcessFile(string file)
    {
        GetLanguageData(oldFolder, file, Language).TryGetPropertyValue("content", out JsonNode? oldContent);

        JsonObject newJson = GetLanguageData(Folder, file, Language);
        JsonNode newContent = newJson["content"]!;

        oldContent ??= newContent.StubContainer();

        return (new SyncContext(oldContent, newContent), newJson["readers"]);
    }

    protected override JsonNode ProcessPrimitive(SyncContext context)
    {
        (JsonNode oldContent, JsonNode newContent) = context;
        JsonPatch patch = newContent.CreatePatch(oldContent);
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
