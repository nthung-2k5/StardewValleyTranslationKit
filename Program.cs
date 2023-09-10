﻿// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewValley.Formats;
using SVTranslation.Process.Export;
using SVTranslation.Process.Import;

//GlobalPath.OldVersion = args[0];
//GlobalPath.NewVersion = args[1];

//var trans = new NewContentTranslator(false);
//foreach (var file in Directory.GetFiles(GlobalPath.NewVersion, "*.es-ES.json", SearchOption.AllDirectories))
//{
//    trans.Process(Path.GetRelativePath(GlobalPath.NewVersion, file.Replace(".es-ES.json", null)));
//}
//trans.WriteJson();

JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    NullValueHandling = NullValueHandling.Include,
};

//GlobalPath.OldVersion = args[0];
//GlobalPath.NewVersion = args[1];

//var files = from file in Directory.GetFiles(GlobalPath.NewVersion, "*.es-ES.json", SearchOption.AllDirectories)
//            select Path.GetRelativePath(GlobalPath.NewVersion, file.Replace(".es-ES.json", null));

// export
var jp = new NewContentTranslator(args[0], args[1], false, false);
jp.Export("ja-JP");

var chingchong = new NewContentTranslator(args[0], args[1], false, false);
chingchong.Export("zh-CN");

var vn = new NewContentTranslator(args[0], args[1], false, false);
vn.Export("es-ES");

var eng = new NewContentTranslator(args[0], args[1], true, true, (folder, json) => new CsvFormat(folder, json, new("Japanese", jp.LogJson), new("Chinese", chingchong.LogJson), new("VN", vn.LogJson)));
eng.Export();

//var sync = new StardewSync(args[0], args[1]);
//sync.Export();

//var import = new StardewUpdate(args[0], args[0] + "_test")
//{
//    GetImportFormat = (folder) => new CsvFormat(folder, new())
//};
//import.Import();