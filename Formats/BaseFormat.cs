using Newtonsoft.Json.Linq;

namespace StardewValley.Formats;
public abstract class BaseFormat
{
    protected BaseFormat(string path, JObject content)
    {
        FormatPath = path;
        Content = content;
    }
    public string FormatPath { get; set; }
    public JObject Content { get; set; }
    public abstract void Export();
    public abstract void Import();
}
