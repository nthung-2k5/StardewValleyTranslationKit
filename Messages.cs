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
        return mess.Any() ? mess : script;
    }

    public static string ApplyMessages(this string script, string[] messages)
    {
        int i = 0;
        return regex.Replace(script, (Match match) =>
        {
            i++;
            return messages[i - 1];
        });
    }
}
