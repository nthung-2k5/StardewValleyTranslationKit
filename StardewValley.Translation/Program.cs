// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using System.Text.Json.Serialization;
using StardewValley.Translation.Formats;
using StardewValley.Translation.Process;

var trans = new ExtractTranslation(args[0], args[1], new JsonFormat(), fullExport: true);
trans.Process();
//Console.WriteLine("Hello world");
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