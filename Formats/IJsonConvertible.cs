using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVTranslation.Formats;
public interface IJsonConvertible
{
    void ToFormat();
    void ToJson();
}
