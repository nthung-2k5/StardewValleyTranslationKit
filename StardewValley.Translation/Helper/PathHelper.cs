namespace StardewValley.Translation.Helper;

public static class PathHelper
{
    public static string RemoveAllExtensions(string path)
    {
        int pos = path.LastIndexOf('.');

        if (pos == -1)
        {
            return path;
        }
        
        int dot = pos;
        while (pos >= 0)
        {
            if (path[pos] == '.')
            {
                dot = pos;
            }
            else if (path[pos] == Path.DirectorySeparatorChar || path[pos] == Path.AltDirectorySeparatorChar)
            {
                break;
            }    
            pos--;
        }

        return path[..dot];
    }
}
