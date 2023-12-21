using StardewValley.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SVTranslation.Process.Export;
public abstract class BaseExportProcess
{
    protected BaseExportProcess(string old, string @new) => (OldFolder, NewFolder) = (old, @new);
    public abstract void Export(string language = "es-ES");
    public abstract void ExportSingle(string @base, string language = "es-ES");
    protected void ExportType(string filename, JsonElement oldContent, JsonElement newContent, JsonElement referenceContent, ClassEnum? @class = null)
    {
        if (@class is ClassEnum cls)
        {
            ExportClass(filename, referenceContent, cls);
        }
        else
        {
            ExportPrimitive(filename, oldContent, newContent, referenceContent);
        }
    }
    protected abstract void ExportPrimitive(string filename, JsonElement oldContent, JsonElement newContent, JsonElement referenceContent);
    protected abstract void ExportClass(string filename, JsonElement referenceContent, ClassEnum @class);
    protected string NewFolder { get; set; }
    protected string OldFolder { get; set; }
}
