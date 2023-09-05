using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVTranslation.Process.Export;
public abstract class BaseExportProcess
{
    protected BaseExportProcess(string old, string @new) => (OldFolder, NewFolder) = (old, @new);
    public abstract void Export(string language = "es-ES");
    public abstract void ExportSingle(string @base, string language = "es-ES");
    protected void ExportType(string filename, JToken oldContent, JToken newContent, JToken referenceContent, bool @class)
    {
        if (!@class)
        {
            ExportPrimitive(filename, oldContent, newContent, referenceContent);
        }
        else
        {
            ExportClass(filename, referenceContent);
        }
    }
    protected abstract void ExportPrimitive(string filename, JToken oldContent, JToken newContent, JToken referenceContent);
    protected abstract void ExportClass(string filename, JToken referenceContent);
    protected string NewFolder { get; set; }
    protected string OldFolder { get; set; }
}
