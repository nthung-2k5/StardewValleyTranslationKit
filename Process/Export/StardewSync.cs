using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewValley;
using StardewValley.Formats;
using SVTranslation.Formats;
using static StardewValley.JsonHelper;

namespace SVTranslation.Process.Export;
public class StardewSync : BaseExportProcess, IExportSingle
{
    public StardewSync(string old, string @new) : base(old, @new)
    {
    }

    public override void Export(string language = "es-ES")
    {
        foreach (var file in Directory.EnumerateFiles(NewFolder, "*.es-ES.json", SearchOption.AllDirectories))
        {
            Export(Path.GetRelativePath(NewFolder, file.Replace(".es-ES.json", null)));
        }
    }
    public override void ExportSingle(string @base, string language = "es-ES")
    {
        JToken oldContent = GetLanguageDataNoFile(OldFolder, @base, language).content;

        (var newJson, var newFile) = GetLanguageData(NewFolder, @base, language);
        var newContent = newJson.content;

        JToken referenceJson = GetLanguageDataNoFile(NewFolder, @base).content;

        ExportType(@base, oldContent, newContent, referenceJson, IsClass(newJson));

        Write(newFile, newJson);
    }
    public void Write(FileStream fs, JObject json)
    {
        using var exporter = new JsonFormat(fs.Name)
        {
            Content = json
        };
        exporter.Export();
    }
    protected override void ExportPrimitive(string filename, JToken oldContent, JToken newContent, JToken referenceContent)
    {
        if (newContent is JObject newDict)
        {
            foreach (var (k, _) in newDict)
            {
                if (oldContent is not null && (oldContent as JObject).ContainsKey(k))
                {
                    newDict[k] = oldContent[k];
                }
            }
        }
#if SV_1_6
        else if (newContent is JArray && referenceContent is JArray referenceArr)
        {
            if (WriteNewContent)
            {
                LogJson[filename] = reference;
            }
            if (PrintNewContent)
            {
                foreach (var text in referenceArr.ToObject<string[]>())
                {
                    AlertNewText(filename, text);
                }
            }
        }
#endif
    }

    protected override void ExportClass(string filename, JToken referenceContent)
    {
#if SV_1_6
#endif
    }
}
