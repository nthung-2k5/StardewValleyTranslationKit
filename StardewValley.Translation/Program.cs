// See https://aka.ms/new-console-template for more information
using StardewValley.Formats;
using System.Text.Json;
using System.Text.Json.Serialization;

//GlobalPath.OldVersion = args[0];
//GlobalPath.NewVersion = args[1];

//var trans = new NewContentTranslator(false);
//foreach (var file in Directory.GetFiles(GlobalPath.NewVersion, "*.es-ES.json", SearchOption.AllDirectories))
//{
//    trans.Process(Path.GetRelativePath(GlobalPath.NewVersion, file.Replace(".es-ES.json", null)));
//}
//trans.WriteJson();

JsonSerializerOptions options = new()
{
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};

//GlobalPath.OldVersion = args[0];
//GlobalPath.NewVersion = args[1];

//var files = from file in Directory.GetFiles(GlobalPath.NewVersion, "*.es-ES.json", SearchOption.AllDirectories)
//            select Path.GetRelativePath(GlobalPath.NewVersion, file.Replace(".es-ES.json", null));

// export
//var jp = new NewContentTranslator(args[0], args[1], false, false);
//jp.Export("ja-JP");

//var chingchong = new NewContentTranslator(args[0], args[1], false, false);
//chingchong.Export("zh-CN");

//var eng = new NewContentTranslator(args[0], args[1], true, true, (folder, json) => new CsvFormat(folder, json, new("Japanese", jp.LogJson), new("Chinese", chingchong.LogJson)));
//eng.Export();

//var import = new StardewUpdate(args[0], args[0] + "_test")
//{
//    GetImportFormat = (folder) => new CsvFormat(folder, new())
//};
//import.Import();