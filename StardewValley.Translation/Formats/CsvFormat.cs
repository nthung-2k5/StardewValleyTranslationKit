using CsvHelper;
using SVTranslation.Formats;
using System.Text.RegularExpressions;

namespace StardewValley.Formats;
public class CsvFormat : BaseFormat
{
    public CsvFormat(string path, JObject json, params LanguageContext[] contexts) : base(path, json) => Contexts = contexts;
    public List<TranslationLine> Lines { get; set; } = [];

    public override void Export()
    {
        ToFormat();
        string[] types = ["dialogue", "strings"];
        foreach (var type in types)
        {
            using var file = File.Open(Path.Combine(FormatPath, $"{type}.new.csv"), FileMode.Create, FileAccess.Write);
            using var writer = new CsvWriter(new StreamWriter(file), System.Globalization.CultureInfo.InvariantCulture);

            IEnumerable<TranslationLine> lines = Lines.Where(line => line.Key.Contains(type));
            writer.WriteRecords(lines);
        }

        const int DATA_THRESHOLD = 962;
        IEnumerable<TranslationLine> dataLines = Lines.Where(line => line.Key.Contains("data"));
        for (int i = 0; i < dataLines.Count() / DATA_THRESHOLD + (dataLines.Count() % DATA_THRESHOLD != 0 ? 1 : 0); i++)
        {
            using var file = File.Open(Path.Combine(FormatPath, string.Format("data{0}.new.csv", i > 0 ? $"({i})" : string.Empty)), FileMode.Create, FileAccess.Write);
            using var writer = new CsvWriter(new StreamWriter(file), System.Globalization.CultureInfo.InvariantCulture);
            writer.WriteRecords(dataLines.Skip(DATA_THRESHOLD * i).Take(DATA_THRESHOLD));
        }
    }
    public override void Import()
    {
        foreach (var csv in Directory.EnumerateFiles(FormatPath, "*.new.csv"))
        {
            using var file = File.OpenRead(csv);
            using var reader = new CsvReader(new StreamReader(file), System.Globalization.CultureInfo.InvariantCulture);
            Lines.AddRange(reader.GetRecords<TranslationLine>());
        }
        ToJson();
    }
    public void ToFormat()
    {
        JTokenReader reader = new(Content);
        while (reader.Read())
        {
            if (reader.Value != null && reader.TokenType != Newtonsoft.Json.JsonToken.PropertyName)
            {
                IEnumerable<string> contextStrings = from context in Contexts
                                                     let path = context.Context.SelectToken(reader.Path)
                                                     where path is not null
                                                     select $"{context.Language}: {path}";

                Lines.Add(new(reader.Path, reader.Value.ToString(), string.Join('\n', contextStrings)));
            }
        }
    }

    public void ToJson()
    {
        foreach (var line in Lines)
        {
            Content.AddTokenByPath(line.Key, line.GetTranslation());
            //var token = Content.SelectToken(line.Key);
            //token.Replace(line.GetTranslation());
        }    
    }

    public override void Export(FileStream stream)
    {
        throw new NotImplementedException();
    }

    public override void Import(FileStream stream)
    {
        throw new NotImplementedException();
    }

    //private IEnumerable<TranslationLine> convertRecursive(JToken content, TranslationLine formatLine, IEnumerable<LanguageContext> contexts)
    //{
    //    var tempKey = formatLine.Key;
    //    if (content is JObject obj)
    //    {
    //        foreach (var (k, v) in obj)
    //        {
    //            IEnumerable<LanguageContext> reference = from context in contexts
    //                                                     where context.Context.ToObject<JObject>().ContainsKey(k)
    //                                                     select new LanguageContext(context.Language, context.Context[k]);
    //            formatLine = formatLine with { Key = string.IsNullOrEmpty(tempKey) ? k : string.Join('|', tempKey, k) };
    //            foreach (var line in convertRecursive(v, formatLine, reference))
    //                yield return line;    
    //        }
    //    }
    //    else if (content is JArray array)
    //    {
    //        for (int i = 0; i < array.Count; i++)
    //        {
    //            IEnumerable<LanguageContext> reference;
    //            try
    //            {
    //                reference = from context in contexts where i < context.Context.Count()
    //                            select new LanguageContext(context.Language, context.Context[i]);
    //            }
    //            catch
    //            {
    //                reference = default;
    //            }
    //            formatLine = formatLine with { Key = string.IsNullOrEmpty(tempKey) ? $"[{i}]" : string.Join('|', tempKey, $"[{i}]") };
    //            foreach (var line in convertRecursive(array[i], formatLine, reference))
    //                yield return line;
    //        }    
    //    }
    //    else
    //    {
    //        string str = content.ToObject<string>();
    //        yield return formatLine with { Text = str, Context = string.Join('\n', from context in contexts
    //                                                                               select $"{context.Language}: {context.Context}") };
    //    }    
    //}    
    protected LanguageContext[] Contexts { get; set; }
}
public record TranslationLine(string Key, string Text, string Context, string Translated = "")
{
    public string GetTranslation() => !string.IsNullOrEmpty(Translated) ? Translated : Text;
}
public record LanguageContext(string Language, JToken Context);