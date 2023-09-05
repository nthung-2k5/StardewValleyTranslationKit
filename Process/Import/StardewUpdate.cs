using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewValley;
using StardewValley.Class.Concessions;
using StardewValley.Class.Movies;
using StardewValley.Class.MoviesReactions;
using StardewValley.Formats;
using System.Security.Cryptography;
using static StardewValley.JsonHelper;
using static StardewValley.Messages;

namespace SVTranslation.Process.Import;
public class StardewUpdate: BaseImportProcess
{
    public StardewUpdate(string folder, Func<string, BaseFormat> getImportFormat, string export = null) : base(folder, export) => GetImportFormat = getImportFormat;

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
        ImportType(content, modContent, IsClass(json));
        using var outputJson = File.Create(Path.Combine(ExportFolder, $"{@base}.{language}.json"));
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
                content[k].Replace((v is JArray messArr) ? content[k].ToObject<string>().ApplyMessages(messArr.Cast<string>().ToArray()) : v);
            }
        }
        else if (content is JArray && modContent is JArray modArr)
        {
            // 1.5 only
            content.Replace(modArr);
        }
    }

    protected override void ImportClass(JToken content, JToken modContent)
    {
        //Type @class;
        //if (filename.EndsWith(nameof(Concessions)))
        //{
        //    @class = typeof(Concessions);
        //}
        //else if (filename.EndsWith(nameof(MoviesReactions)))
        //{
        //    @class = typeof(MoviesReactions);
        //}
        //else if (filename.EndsWith(nameof(Movies)))
        //{
        //    @class = typeof(Movies);
        //}
        //else
        //    throw new NotSupportedException("Class not found in 1.5");

        //LogJson[filename] = (JToken)@class.GetMethod("Apply").Invoke(null, new object[] { referenceContent });
    }

    public JObject ModContent { get; set; }
    protected Func<string, BaseFormat> GetImportFormat { get; set; }
}
