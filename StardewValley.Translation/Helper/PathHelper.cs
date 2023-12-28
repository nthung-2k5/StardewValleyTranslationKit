using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVTranslation.Helper;

public static class PathHelper
{
    public static string GetLanguageFile(string folder, string basename, string language = "")
    {
        return Path.ChangeExtension(Path.Combine(folder, basename), language + ".json");
    }
    public static string GetFileNameWithoutExtension(string path)
    {
        int pos = path.LastIndexOf('.');
        int dot = pos;

        if (pos == -1)
        {
            return path;
        }
        while (pos >= 0)
        {
            if (path[pos] == '.')
            {
                dot = pos;
            }
            else if (path[pos] == Path.PathSeparator || path[pos] == Path.AltDirectorySeparatorChar)
            {
                break;
            }    
            pos--;
        }
        return path[dot..pos];
    }
}
