using StardewValley.JsonClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVTranslation.Process.Import;
public abstract class BaseImportProcess
{
    protected BaseImportProcess(string folder, string export = null) => (Folder, ExportFolder) = (folder, export ?? folder);
    public abstract void Import(string language = "es-ES");
    public abstract void ImportSingle(string @base, JToken modContent, string language = "es-ES");
    protected void ImportType(JToken content, JToken modContent, ClassEnum? @class = null)
    {
        if (@class is ClassEnum cls)
        {
            ImportClass(content, modContent, cls);
        }
        else
        {
            ImportPrimitive(content, modContent);
        }
    }
    protected abstract void ReadModContent();
    protected abstract void ImportPrimitive(JToken content, JToken modContent);
    protected abstract void ImportClass(JToken content, JToken modContent, ClassEnum @class);
    protected string ExportFolder { get; set; }
    protected string Folder { get; set; }
}
