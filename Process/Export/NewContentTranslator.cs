using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewValley;
using StardewValley.Class;
using StardewValley.Class.Concessions;
using StardewValley.Class.Movies;
using StardewValley.Class.MoviesReactions;
using StardewValley.Formats;
using SVTranslation.Helper;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using static StardewValley.JsonHelper;

namespace SVTranslation.Process.Export;
public class NewContentTranslator : BaseExportProcess, IExportMulti
{
    public override void Export(string language = "es-ES")
    {
        var files = new HashSet<string>(Directory.EnumerateFiles(NewFolder, "*", SearchOption.AllDirectories).Select(PathHelper.GetFileNameWithoutExtension));
        foreach (var file in files)
        {
            ExportSingle(Path.GetFileName(file), language);
        }
        if (WriteNewContent)
        {
            Write();
        }
    }
    public override void ExportSingle(string @base, string language = "es-ES")
    {
        JToken oldContent = UseReference ? GetLanguageDataNoFile(OldFolder, @base, language).content : new JObject();

        JObject newJson = GetLanguageDataNoFile(NewFolder, @base, language);

        if (!newJson.HasValues) return;

        var newContent = newJson["content"];

        JToken referenceJson = UseReference ? GetLanguageDataNoFile(NewFolder, @base).content : newContent;

        ExportType(@base, oldContent, newContent, referenceJson, IsClass(@base));
    }
    public void Write()
    {
        var exporter = GetExportFormat(NewFolder, LogJson);
        exporter.Export();
    }
    public NewContentTranslator(string old, string @new, bool writeContent, bool reference, Func<string, JsonObject, IFormat> getExportFormat = null, bool logConsole = false) : base(old, @new)
    {
        WriteNewContent = writeContent;
        UseReference = reference;
        GetExportFormat = getExportFormat;
        PrintNewContent = logConsole;

        if (writeContent && getExportFormat is null)
        {
            throw new ArgumentNullException(nameof(getExportFormat));
        }    
    }

    protected override void ExportPrimitive(string filename, JsonElement? oldContent, JsonElement newContent, JsonElement? referenceContent)
    {
        var reference = referenceContent ?? newContent;
        if (newContent.ValueKind == JsonValueKind.Object)
        {
            foreach (var kv in newContent.EnumerateObject())
            {
                if (oldContent is null || !(oldContent as JObject).ContainsKey(k))
                {
                    string text = reference[k].ToObject<string>();
                    if (PrintNewContent)
                    {
                        AlertNewText(filename, text, k);
                    }
                    JToken log = LogJson[filename] ?? (LogJson[filename] = new JObject());
                    log[k] = text.Contains("/message") ? new JArray(text.GetMessages()) : reference[k];
                }
            }
        }
        else if (newContent.ValueKind == JsonValueKind.Array && reference.ValueKind == JsonValueKind.Array)
        {
            LogJson[filename] = reference;
            if (PrintNewContent)
            {
                foreach (var text in reference.EnumerateArray())
                {
                    AlertNewText(filename, text.GetString());
                }
            }
        }
    }

    protected override void ExportClass(string filename, JsonElement referenceContent, ClassEnum @class)
    {
        Type type = @class switch
        {
            ClassEnum.Concessions => typeof(Concessions),
            ClassEnum.MoviesReactions => typeof(MoviesReactions),
            ClassEnum.Movies => typeof(Movies),
            _ => throw new NotSupportedException("Class not found in 1.5"),
        };

        LogJson[filename] = (JsonNode)type.GetMethod("Generate").Invoke(null, new object[] { referenceContent });
    }

    public JsonObject LogJson { get; set; } = new();
    public static void AlertNewText(string filename, string value, string key = null)
    {
        Console.WriteLine("New content found in file: {0}", filename);
        if (key is not null)
        {
            Console.WriteLine("Key: {0}", key);
        }
        Console.WriteLine("Text: {0}", value);
        Console.WriteLine("Writing to log file...");
        Console.WriteLine();
    }
    public bool WriteNewContent { get; set; }
    public bool PrintNewContent { get; set; }
    public bool UseReference { get; set; }
    protected Func<string, JsonObject, IFormat> GetExportFormat { get; set; }
}
