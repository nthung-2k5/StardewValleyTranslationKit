using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace StardewValley.Translation.JsonClass;

public class ScriptMessage(IEnumerable<string> list) : List<string>(list)
{
    public ScriptMessage(string script): this(Extract(script)) {}
    public void Apply(ref string script)
    {
        string[] split = script.Split('/');
        int count = 0;
        for (int i = 0; i < split.Length; i++)
        {
            if (split[i].StartsWith("message"))
            {
                split[i] = $"""
                            message "{this[count]}"
                            """;
                count++;
            }
        }

        script = string.Join('/', split);
    }

    private static IEnumerable<string> Extract(string script)
    {
        return from split in script.Split('/')
               where split.StartsWith("message")
               select split[split.IndexOf('\"')..split.LastIndexOf('\"')];
    }
    
    public static ScriptMessage? From(string script)
    {
        var message = new ScriptMessage(script);
        return message.Count > 0 ? message : null;
    }
}
