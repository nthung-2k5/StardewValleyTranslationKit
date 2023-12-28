using StardewValley.JsonClass;
using SVTranslation.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SVTranslation.Process.Export;
public abstract class BaseExportProcess
{
    public string Language { get; init; } = "es-ES";
    public const string ReferenceLanguage = ""; // English
    protected BaseExportProcess(string old, string @new) => (OldFolder, NewFolder) = (old, @new);
    public abstract void ExportFolder();
    public abstract void ExportSingle(string @base);
    protected void ExportType(string filename, JsonNode? oldContent, JsonNode newContent, JsonNode referenceContent, string? typeName = null)
    {
        if (typeName != null)
        {
            ExportClass(filename, newContent, referenceContent, typeName);
        }
        else
        {
            ExportPrimitive(filename, oldContent, newContent, referenceContent);
        }
    }
    protected abstract void ExportPrimitive(string filename, JsonNode? oldContent, JsonNode newContent, JsonNode referenceContent);
    protected void ExportClass(string filename, JsonNode newContent, JsonNode referenceContent, string typeName)
    {
        
    }

    protected readonly string NewFolder;
    protected readonly string OldFolder;

    protected static string[] GetBaseFileNames(string folder)
    {
        var files = new HashSet<string>(Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories).Select(PathHelper.GetFileNameWithoutExtension));
        return [..files];
    }
}
