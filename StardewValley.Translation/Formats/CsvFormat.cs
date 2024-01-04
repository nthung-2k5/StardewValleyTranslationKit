using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using CsvHelper;
using Json.Path;
using StardewValley.Translation.Helper;

namespace StardewValley.Translation.Formats;

public partial class CsvFormat(params LanguageContext[] contexts) : BaseFormat, ICsvFormat
{
    public List<TranslationLine> Lines { get; } = [];

    protected override void Export(FileStream stream)
    {
        ToCsv();
        // string[] types = ["dialogue", "strings"];
        // foreach (string type in types)
        // {
        //     using var file = File.Create(Path.Combine(path, $"{type}.new.csv"));
        //     using var writer = new CsvWriter(new StreamWriter(file), System.Globalization.CultureInfo.InvariantCulture);
        //
        //     var lines = Lines.Where(line => line.Key.Contains(type));
        //     writer.WriteRecords(lines);
        // }
        //
        // const int DataThreshold = 1000;
        // var dataLines = Lines.Where(line => line.Key.Contains("data")).ToArray();
        // for (int i = 0; i < dataLines.Length / DataThreshold + (dataLines.Length % DataThreshold != 0 ? 1 : 0); i++)
        // {
        //     using var file = File.Create(Path.Combine(path, $"data{(i > 0 ? $"({i})" : string.Empty)}.new.csv"));
        //     using var writer = new CsvWriter(new StreamWriter(file), System.Globalization.CultureInfo.InvariantCulture);
        //     writer.WriteRecords(dataLines.Skip(DataThreshold * i).Take(DataThreshold));
        // }
        using var writer = new CsvWriter(new StreamWriter(stream), CultureInfo.InvariantCulture);
        writer.WriteRecords(Lines);
    }

    protected override void Import(FileStream stream)
    {
        // foreach (var csv in Directory.EnumerateFiles(path, "*.new.csv"))
        // {
        //     using var file = File.OpenRead(csv);
        //     using var reader = new CsvReader(new StreamReader(file), System.Globalization.CultureInfo.InvariantCulture);
        //     Lines.AddRange(reader.GetRecords<TranslationLine>());
        // }
        using var reader = new CsvReader(new StreamReader(stream), CultureInfo.InvariantCulture);
        Lines.AddRange(reader.GetRecords<TranslationLine>());
        ToJson();
    }

    private void ToCsv()
    {
        JsonPath allPath = JsonPath.Parse("$..*");
        NodeList nodes = allPath.Evaluate(Content).Matches!;

        foreach (Node node in nodes)
        {
            JsonNode value = node.Value!;

            if (node.Value!.GetValueKind() != JsonValueKind.String)
            {
                continue;
            }

            IEnumerable<string> contextStrings = from context in contexts
                                                 let contextNode = context.Context.SelectToken(node.Location)
                                                 where contextNode is not null
                                                 select $"{context.Language}: {contextNode.GetValue<string>()}";

            Lines.Add(new TranslationLine(value.GetPath()[1..].Replace("\\", @"\\"), value.GetValue<string>(),
                                          string.Join('\n', contextStrings)));
        }
        // var newtonsoftJson = JToken.Parse(Content!.ToJsonString());
        // var reader = new JTokenReader(newtonsoftJson);
        // while (reader.Read())
        // {
        //     if (reader.Value != null && reader.TokenType != Newtonsoft.Json.JsonToken.PropertyName)
        //     {
        //         IEnumerable<string> contextStrings = from context in contexts
        //                                              let node = context.Context.SelectToken(reader.Path)
        //                                              where node is not null
        //                                              select $"{context.Language}: {node.GetValue<string>()}";
        //     
        //         Lines.Add(new TranslationLine(reader.Path, reader.ReadAsString()!, string.Join('\n', contextStrings)));
        //     }
        // }
        // reader.Close();
    }

    private void ToJson()
    {
        foreach (TranslationLine line in Lines)
        {
            Content.AddTokenByPath("$" + line.Key.Replace(@"\\", "\\"), line.GetTranslation());
        }
    }
}

public record TranslationLine(string Key, string Text, string Context, string Translated = "")
{
    public string GetTranslation() => !string.IsNullOrEmpty(Translated) ? Translated : Text;
}

public record LanguageContext(string Language, JsonObject Context);
