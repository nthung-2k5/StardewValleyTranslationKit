using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Patch;
using StardewValley.Translation.Formats;
using StardewValley.Translation.Helper;
using StardewValley.Translation.JsonClass;
using static StardewValley.Translation.Helper.JsonFileHelper;

namespace StardewValley.Translation.Process;

public record TranslationContext(JsonNode OldContent, JsonNode Content, JsonNode ReferenceContent)
    : ProcessContext(Content);

public class ExtractTranslation(
    string oldFolder,
    string newFolder,
    IFormat? format,
    string referenceLanguage = "",
    bool fullExport = false,
    string? exportFolder = null) : BaseProcess<TranslationContext>(newFolder, exportFolder), IFormatVisitor
{
    private readonly ConcurrentDictionary<string, JsonNode> jsonLog = [];
    private ImmutableSortedDictionary<string, JsonNode>? sortedJsonLog;
    
    public void VisitIJsonFormat(IJsonFormat element)
    {
        element.Content = JsonSerializer.SerializeToNode(sortedJsonLog)?.AsObject();
        element.Export(Path.Combine(ExportFolder, "content.new.json"));
    }

    public void VisitICsvFormat(ICsvFormat element)
    {
        string[] folders = ["dialogue", "strings", "data"];

        foreach (string folder in folders)
        {
            element.Lines.Clear();
            var obj = sortedJsonLog!.Where(kv => kv.Key.Contains(folder));
            JsonObject folderObj = JsonSerializer.SerializeToNode(obj.ToDictionary(kv => kv.Key, kv => kv.Value))!.AsObject();
            element.Content = folderObj;
            element.Export(Path.Combine(ExportFolder, $"{folder}.new.csv"));
        }
    }

    protected override (TranslationContext, JsonNode?) ProcessFile(string file)
    {
        JsonObject newJson = GetLanguageData(Folder, file, Language);
        JsonNode newContent = newJson["content"]!;

        GetLanguageData(oldFolder, file, Language).TryGetPropertyValue("content", out JsonNode? oldContent);

        bool useReference = File.Exists(GetLanguageFile(Folder, file, referenceLanguage));

        if (fullExport || oldContent is null)
        {
            oldContent = newContent.StubContainer();
        }

        JsonNode referenceJson = useReference ? GetLanguageData(Folder, file, referenceLanguage)["content"]! : newContent;

        return (new TranslationContext(oldContent, newContent, referenceJson), newJson["readers"]);
    }

    protected override JsonNode ProcessPrimitive(TranslationContext context)
    {
        (JsonNode old, JsonNode @new, JsonNode reference) = context;
        List<PatchOperation> patches = [];

        JsonNode fileLog = @new.StubContainer();

        foreach (PatchOperation patch in old.CreatePatch(@new).Operations.Where(op => op.Op == OperationType.Add))
        {
            _ = patch.Path.TryEvaluate(reference, out JsonNode? referenceNode);

            string text = referenceNode!.GetValue<string>();
            patches.Add(PatchOperation.Add(patch.Path, Script.HasText(text) ? JsonValue.Create(Script.From(text)) : text));
        }

        var truePatch = new JsonPatch(patches);

        return truePatch.Apply(fileLog).Result!;
    }

    protected override JsonNode ProcessClass(TranslationContext context, string type) =>
        ClassTranslation.Convert(context.ReferenceContent, type);

    protected override void OnFileProcessed((string file, JsonNode content) e)
    {
        base.OnFileProcessed(e);
        jsonLog[e.file] = e.content;
    }

    protected override void OnAllFileProcessed()
    {
        base.OnAllFileProcessed();
        sortedJsonLog = jsonLog.ToImmutableSortedDictionary();
        format?.Accept(this);
    }
}
