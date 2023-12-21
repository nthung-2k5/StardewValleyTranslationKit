using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVTranslation.Process.Import;
public interface IImportSingle
{
    void Read(FileStream fs, JObject json);
}
public interface IImportMulti
{
    void Read();
}
