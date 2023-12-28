using SVTranslation.Helper;
using System.Text.Json.Nodes;
using StardewValley.GameData.Movies;
using System.Text.RegularExpressions;
using System.Collections.Immutable;
using System.Text.Json;
using StardewValley.Translation.JsonClass;

namespace StardewValley;
#nullable disable
public static partial class JsonHelper
{
    public static JsonObject GetLanguageData(string folder, string basename, string language = null)
        => GetLanguageData(PathHelper.GetLanguageFile(folder, basename, language));

    public static JsonObject GetLanguageData(string path)
    {
        if (!File.Exists(path)) return [];

        using FileStream file = File.OpenRead(path);
        return JsonNode.Parse(file).AsObject();
    }

    public static bool IsClass(JsonArray readers)//, out ClassType? value)
    {
        foreach (var reader in readers)
        {
            var type = reader["type"].GetValue<string>();

            if (!type.Contains("ReflectiveReader")) continue;

            Match res = TypeParsingRegex.Match(type);
            Group genericArgCount = res.Groups["GenericArgCount"];
            Group genericArgs = res.Groups["Arg"];

            if (genericArgCount?.Success != true) // Not a generic, lets short-circuit
            {
                continue;
            }

            string argType = TypeDetailsRegex.Match(genericArgs.Captures[0].Value[2..^2]).Value.Split('.')[^1];
            
            //return ClassType.TryParse(argType, out value);
            return true;
        }
        //value = null;
        return false;
    }
    private static readonly Regex TypeParsingRegex = TypeParse();

    [GeneratedRegex(@"^(?<TypeName>[\w\.]+(?:`(?<GenericArgCount>\d+))?)(?:\[(?:(?<Arg>\[(?>(?<c>\[)|(?<-c>\])|[^[\]]+)+\](?(c)(?!))),?\s*)+\])?", RegexOptions.Compiled)]
    private static partial Regex TypeParse();

    private static readonly Regex TypeDetailsRegex = TypeDetail();

    [GeneratedRegex(@"^\[\[(?<TypeName>.+),(?:.+,){3}.+\]$")]
    private static partial Regex TypeDetail();
}
#nullable restore