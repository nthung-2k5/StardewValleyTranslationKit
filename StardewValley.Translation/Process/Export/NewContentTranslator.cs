using StardewValley;
using StardewValley.JsonClass;
using StardewValley.JsonClass.Concessions;
using StardewValley.JsonClass.Movies;
using StardewValley.JsonClass.MoviesReactions;
using StardewValley.Formats;
using SVTranslation.Helper;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using static StardewValley.JsonHelper;

namespace SVTranslation.Process.Export;
public class NewContentTranslator : BaseProcess, IExportMulti
{
    public override void Process()
    {
        foreach (var file in GetBaseFileNames(NewFolder))
        {
            ProcessFile(Path.GetFileName(file));
        }

        if (WriteNewContent)
        {
            Write();
        }
    }
    public override void ProcessFile(string @base)
    {
        JToken oldContent = UseReference ? GetLanguageDataNoFile(OldFolder, @base, language).content : new JObject();

        JObject newJson = GetLanguageDataNoFile(NewFolder, @base, language);

        if (!newJson.HasValues) return;

        var newContent = newJson["content"];

        JToken referenceJson = UseReference ? GetLanguageDataNoFile(NewFolder, @base).content : newContent;

        ExportType(@base, oldContent, newContent, referenceJson, IsClass(@base));
    }
    private void Write()
    {
        ExportFormat.Content = LogJson;
        ExportFormat.Export(NewF)
    }
    public NewContentTranslator(string old, string @new, bool writeContent, bool reference, BaseFormat format, bool logConsole = false) : base(old, @new)
    {
        WriteNewContent = writeContent;
        UseReference = reference;
        ExportFormat = format;
        PrintNewContent = logConsole;
        if (!format.Mode.HasFlag(FormatMode.Export))
        {
            throw new ArgumentException("Unable to export", nameof(format));
        }
    }

    protected override void ProcessPrimitive(string filename, JsonNode oldContent, JsonNode newContent, JsonNode referenceContent)
    {
        var reference = referenceContent ?? newContent;
        if (newContent.GetValueKind() == JsonValueKind.Object)
        {
            foreach (var kv in newContent.AsObject())
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
        else if (newContent.GetValueKind() == JsonValueKind.Array && reference.GetValueKind() == JsonValueKind.Array)
        {
            LogJson[filename] = reference;
            if (PrintNewContent)
            {
                foreach (var text in reference.AsArray())
                {
                    AlertNewText(filename, text!.GetValue<string>());
                }
            }
        }
    }

    protected override void ProcessClass(string filename, JsonNode referenceContent, ClassEnum @class)
    {
        Type type = @class switch
        {
            ClassEnum.Concessions => typeof(JsonConcessionItemData),
            ClassEnum.MoviesReactions => typeof(JsonMovieCharacterReaction),
            ClassEnum.Movies => typeof(JsonMovieData),
            _ => throw new NotSupportedException("Class not found in 1.5"),
        };

        LogJson[filename] = (JsonNode)type.GetMethod("Generate").Invoke(null, new object[] { referenceContent });
    }

    public JsonObject LogJson { get; set; } = [];
    public static void AlertNewText(string filename, string value, string? key = null)
    {
        Console.WriteLine("New content found in file: {0}", filename);
        if (key != null)
        {
            Console.WriteLine("Key: {0}", key);
        }
        Console.WriteLine("Text: {0}", value);
        Console.WriteLine("Writing to log file...");
        Console.WriteLine();
    }
    public bool WriteNewContent { get; }
    public bool PrintNewContent { get; }
    public bool UseReference { get; }
    protected BaseFormat ExportFormat;
}
