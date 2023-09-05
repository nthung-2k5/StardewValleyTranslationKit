using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVTranslation.Process.Export;
public interface IExportSingle
{
    void Write(FileStream fs, JObject json);
}
public interface IExportMulti
{
    void Write();
}