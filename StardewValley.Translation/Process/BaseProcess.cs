using StardewValley.JsonClass;
using SVTranslation.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SVTranslation.Process;
public abstract class BaseProcess
{
    public string Language { get; init; } = "es-ES";
    public const string ReferenceLanguage = ""; // English
    protected BaseProcess(string old, string @new) => (OldFolder, NewFolder) = (old, @new);
    public abstract void Process();
    public abstract void ProcessFile(string @base);
    protected void ProcessType(string filename, JsonNode? oldContent, JsonNode newContent, JsonNode referenceContent, ClassEnum? @class = null)
    {
        if (@class is ClassEnum cls)
        {
            ProcessClass(filename, referenceContent, cls);
        }
        else
        {
            ProcessPrimitive(filename, oldContent, newContent, referenceContent);
        }
    }
    protected abstract void ProcessPrimitive(string filename, JsonNode? oldContent, JsonNode newContent, JsonNode referenceContent);
    protected abstract void ProcessClass(string filename, JsonNode referenceContent, ClassEnum @class);

    protected readonly string NewFolder;
    protected readonly string OldFolder;

    protected static string[] GetBaseFileNames(string folder)
    {
        var files = new HashSet<string>(Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories).Select(PathHelper.GetFileNameWithoutExtension));
        return [.. files];
    }
}

