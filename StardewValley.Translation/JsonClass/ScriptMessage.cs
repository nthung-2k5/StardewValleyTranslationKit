using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StardewValley.Translation.JsonClass;
public partial class ScriptMessage: List<string>
{
    public void Apply(ref string script)
    {
        int i = 0;
        script = regex.Replace(script, (Match match) =>
        {
            i++;
            return $" \"{this[i - 1]}\"";
        });
    }

    //public static implicit operator ScriptMessage(string script) => new(regex.Matches(script).Select(x => x.Groups[1].Value));

    #region Compiled regex

    private static Regex regex = MyRegex();

    [GeneratedRegex("(?:(?<=/message)|(?<=/メッセージ)).*?(?:「|\\\")(.*?)(?:」|\\\")")]
    private static partial Regex MyRegex();
    #endregion
}
