using StardewValley;
using StardewValley.JsonClass;
using StardewValley.Formats;
using SVTranslation.Formats;
using static StardewValley.JsonHelper;
using System.Text.Json;
using System.Text.Json.Nodes;
using SVTranslation.Helper;

namespace SVTranslation.Process.Export;
public class StardewSync(string old, string @new) : BaseProcess(old, @new)
{
    public override void Process()
    {
        foreach (var file in Directory.EnumerateFiles(NewFolder, "*.es-ES.json", SearchOption.AllDirectories))
        {
            ExportSingle(Path.GetRelativePath(NewFolder, file.Replace(".es-ES.json", null)));
        }
    }
    public override void ProcessFile(string @base)
    {
        var oldFile = PathHelper.GetLanguageFile(OldFolder, @base, Language);
        var newFile = PathHelper.GetLanguageFile(NewFolder, @base, Language);

        JsonNode? oldContent = null;
        GetLanguageData(oldFile)?.TryGetPropertyValue("content", out oldContent);

        JsonNode newJson = GetLanguageData(newFile);
        var newContent = newJson["content"]!;

        JsonNode referenceJson = GetLanguageData(NewFolder, @base, ReferenceLanguage)["content"]!;

        ProcessType(@base, oldContent, newContent, referenceJson, IsClass(@base));
        Write(newFile, newJson);
    }
    private static void Write(string file, JsonNode json)
    {
        var exporter = new JsonFormat(json);
        exporter.Export(file);
    }

    protected override void ProcessPrimitive(string filename, JsonNode? oldContent, JsonNode newContent, JsonNode referenceContent)
    {
        if (oldContent == null) return;

        if (newContent.GetValueKind() == JsonValueKind.Object)
        {
            var newKeys = newContent.AsObject().Select(p => p.Key);
            var oldKeys = oldContent.AsObject().Select(p => p.Key);

            var keys = newKeys.Intersect(oldKeys);

            foreach (var key in keys)
            {
                newContent[key]!.ReplaceWith(oldContent[key]);
            }
        }
#if SV_1_6
        else if (newContent.GetValueKind() == JsonValueKind.Array)
        {
            
        }
#endif
    }
    protected override void ProcessClass(string filename, JsonNode referenceContent, ClassEnum @class)
    {
#if SV_1_6
#endif
    }
}
