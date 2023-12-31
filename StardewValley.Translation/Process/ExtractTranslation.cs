using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Patch;
using StardewValley.Translation.Formats;
using StardewValley.Translation.Helper;
using StardewValley.Translation.JsonClass;
using static StardewValley.Translation.Helper.JsonFileHelper;

namespace StardewValley.Translation.Process;
public record TranslationContext(JsonNode OldContent, JsonNode Content, JsonNode ReferenceContent) : ProcessContext(Content);

public class ExtractTranslation(string oldFolder, string newFolder, IFormat? format, string referenceLanguage = "", bool fullExport = false): BaseProcess<TranslationContext>(newFolder), IFormatVisitor
{
    protected override (TranslationContext, JsonNode?) ProcessFile(string file)
    {
        var newJson = GetLanguageData(Folder, file, Language);
        var newContent = newJson["content"]!;
        
        GetLanguageData(oldFolder, file, Language).TryGetPropertyValue("content", out var oldContent);
        
        bool useReference = File.Exists(GetLanguageFile(Folder, file, referenceLanguage));
        if (fullExport || oldContent is null)
        {
            oldContent = newContent.StubContainer();
        }

        var referenceJson = useReference ? GetLanguageData(Folder, file, referenceLanguage)["content"]! : newContent;

        return (new TranslationContext(oldContent, newContent, referenceJson), newJson["readers"]);
    }

    protected override JsonNode ProcessPrimitive(TranslationContext context)
    {
        var (old, @new, reference) = context;
        List<PatchOperation> patches = [];

        var fileLog = @new.StubContainer();
        
        foreach (var patch in old.CreatePatch(@new).Operations.Where(op => op.Op == OperationType.Add))
        {
            _ = patch.Path.TryEvaluate(reference, out var referenceNode);

            string text = referenceNode!.GetValue<string>();
            patches.Add(PatchOperation.Add(patch.Path, Script.HasText(text) ? JsonValue.Create(Script.From(text)) : text));
        }

        var truePatch = new JsonPatch(patches);
        return truePatch.Apply(fileLog).Result!;
    }

    protected override JsonNode ProcessClass(TranslationContext context, string type)
    {
        return ClassTranslation.Convert(context.ReferenceContent, type);
    }

    protected override void OnFileProcessed((string file, JsonNode content) e)
    {
        base.OnFileProcessed(e);
        jsonLog[e.file] = e.content;
    }

    protected override void OnAllFileProcessed()
    {
        base.OnAllFileProcessed();
        format?.Accept(this);
    }
    
    private readonly JsonObject jsonLog = [];
    // private static void AlertNewText(string filename, string value, string? key = null)
    // {
    //     Console.WriteLine("New content found in file: {0}", filename);
    //     if (key is not null)
    //     {
    //         Console.WriteLine("Key: {0}", key);
    //     }
    //     Console.WriteLine("Text: {0}", value);
    //     Console.WriteLine("Writing to log file...");
    //     Console.WriteLine();
    // }
    public void VisitIJsonFormat(IJsonFormat element)
    {
        element.Content = jsonLog;
        element.Export(Path.Combine(Folder, "content.new.json"));
    }

    public void VisitICsvFormat(ICsvFormat element)
    {
        string[] folders = ["dialogue", "strings", "data"];
        foreach (string folder in folders)
        {
            var obj = jsonLog.Where(kv => kv.Key.Contains(folder));
            var folderObj = JsonSerializer.SerializeToNode(obj.ToDictionary(kv => kv.Key, kv => kv.Value));
            element.Content = folderObj;
            element.Export(Path.Combine(Folder, $"{folder}.new.csv"));
        }
        
        // const int DataThreshold = 1000;
        // var dataLines = Lines.Where(line => line.Key.Contains("data")).ToArray();
        // for (int i = 0; i < dataLines.Length / DataThreshold + (dataLines.Length % DataThreshold != 0 ? 1 : 0); i++)
        // {
        //     using var file = File.Create(Path.Combine(path, $"data{(i > 0 ? $"({i})" : string.Empty)}.new.csv"));
        //     using var writer = new CsvWriter(new StreamWriter(file), System.Globalization.CultureInfo.InvariantCulture);
        //     writer.WriteRecords(dataLines.Skip(DataThreshold * i).Take(DataThreshold));
        // }
    }
}
