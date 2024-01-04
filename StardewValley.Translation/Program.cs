// See https://aka.ms/new-console-template for more information

using StardewValley.Translation.Formats;
using StardewValley.Translation.Process;

(string Old, string New) folder = (args[1], args[2]);
IFormat format = args[3] switch
                    {
                        "json" => new JsonFormat(),
                        "csv" => new CsvFormat(),
                        _ => throw new ArgumentException()
                    };
string? export = null;

if (args.Length > 3)
{
    export = args[3];
}
IProcess process;
switch (args[0])
{
    case "import":
    case "-i":
        process = new StardewUpdate(folder.Old, folder.New, format, export);
        break;
    case "extract":
    case "-e":
        process = new ExtractTranslation(folder.Old, folder.New, format, fullExport: true, exportFolder: export);
        break;
    case "sync":
    case "-s":
        process = new SyncToNewVersion(folder.Old, folder.New, export);
        break;
    default:
        throw new NotImplementedException();
}

process.Process();