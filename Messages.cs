using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace StardewValley;
public static partial class Messages
{
    private static Regex regex = MyRegex();

    [GeneratedRegex("(?:(?<=/message)|(?<=/メッセージ)).*?(?:「|\\\")(.*?)(?:」|\\\")")]
    private static partial Regex MyRegex();
    public static string[] GetMessages(this string script)
    {
        return regex.Matches(script).Select(x => x.Groups[1].Value).ToArray();
    }
    public static dynamic GetMessagesDynamic(this string script)
    {
        var mess = GetMessages(script);
        if (mess.Any())
        {
            return mess;
        }    
        else
        {
            return string.IsNullOrEmpty(script) ? null : script;
        }
    }

    public static string ApplyMessages(this string script, string[] messages)
    {
        int i = 0;
        return regex.Replace(script, (Match match) =>
        {
            i++;
            return " \"" + messages[i - 1] + "\"";
        });
    }
    public static string ApplyMessages(this JToken script, JToken messages)
        => script.Value<string>().ApplyMessages(messages.ToObject<string[]>());
    public static string ApplyMessagesDynamic(this JToken script, JToken messages)
    {
        if (messages is JArray arr)
        {
            return ApplyMessages(script, arr);
        }    
        else
        {
            return messages.ToObject<string>();
        }    
    }
}
