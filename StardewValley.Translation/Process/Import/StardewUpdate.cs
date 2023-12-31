using static StardewValley.Translation.JsonHelper;

namespace StardewValley.Translation.Process.Import;
public class StardewUpdate: BaseImportProcess
{
    public StardewUpdate(string folder, string export = null) : base(folder, export) { }

    public override void Import(string language = "es-ES")
    {
        ReadModContent();
        foreach (var file in ModContent)
        {
            ImportSingle(file.Key, file.Value, language);
        }
    }

    public override void ImportSingle(string @base, JToken modContent, string language = "es-ES")
    {
        JObject json = GetLanguageDataNoFile(Folder, @base, language);
        JToken content = json["content"];

        ImportType(content, modContent, IsClass(@base));
        var file = Path.Combine(ExportFolder, $"{@base}.{language}.json");
        Directory.CreateDirectory(Path.GetDirectoryName(file));
        using var outputJson = File.Create(file);
        Write(outputJson, json);
    }
    protected static void Write(FileStream file, JObject content)
    {
        using StreamWriter sw = new(file);
        using JsonTextWriter jw = new(sw);

        jw.Formatting = Formatting.Indented;
        jw.IndentChar = ' ';
        jw.Indentation = 4;

        content.WriteTo(jw);
    }    

    protected override void ReadModContent()
    {
        var format = GetImportFormat(Folder);
        format.Import();
        ModContent = format.Content;
    }

    protected override void ImportPrimitive(JToken content, JToken modContent)
    {
        if (content is JObject && modContent is JObject mod)
        {
            foreach (var (k, v) in mod)
            {
                content[k].Replace((v is JArray messArr) ? content[k].Value<string>().ApplyMessages(messArr.ToObject<string[]>()) : v);
            }
        }
        else if (content is JArray && modContent is JArray modArr)
        {
            // 1.5 only
            content.Replace(modArr);
        }
    }

    protected override void ImportClass(JToken content, JToken modContent, ClassEnum @class)
    {
        Type type = @class switch
        {
            ClassEnum.Concessions => typeof(JsonConcessionItemData),
            ClassEnum.MoviesReactions => typeof(JsonMovieCharacterReaction),
            ClassEnum.Movies => typeof(JsonMovieData),
            _ => throw new NotSupportedException("Class not found in 1.5"),
        };

        type.GetMethod("Apply").Invoke(null, new object[] { content, modContent });
    }

    public JObject ModContent { get; set; }
    public required Func<string, IFormat> GetImportFormat { get; set; }
}
