using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewValley.Class;
using System.Text.RegularExpressions;

namespace StardewValley;
public static partial class JsonHelper
{
    public static (dynamic, FileStream) GetLanguageData(string folder, string basename, string language = null)
    {
        string filename = Path.ChangeExtension(basename, language) + ".json";
        if (File.Exists(Path.Combine(folder, filename)))
        {
            return GetJsonData(Path.Combine(folder, filename));
        }
        else
        {
            return (new JObject(), null);
        }
    }
    public static dynamic GetLanguageDataNoFile(string folder, string basename, string language = null)
    {
        var data = GetLanguageData(folder, basename, language);
        data.Item2?.Close();
        return data.Item1;
    }

    public static (dynamic, FileStream) GetJsonData(string path)
    {
        FileStream file = File.Open(path, FileMode.Open, FileAccess.ReadWrite);
        using StreamReader reader = new(file);
        return (JObject.Parse(reader.ReadToEnd()), file);
    }

    public static ClassEnum? IsClass(string @base)
    {
        if (@base.EndsWith("Concessions"))
        {
            return ClassEnum.Concessions;
        }
        else if (@base.EndsWith("MoviesReactions"))
        {
            return ClassEnum.MoviesReactions;
        }
        else if (@base.EndsWith("Movies"))
        {
            return ClassEnum.Movies;
        }
        else
        {
            return null;
        }
    }
    //public static void AddTokenByPath(this JToken jToken, string path, object value)
    //{
    //    // "a.b.d[1]['my1.2.4'][4].af['micor.a.ee.f'].ra[6]"
    //    var pathParts = MyRegex1().Split(path)
    //        // > { "a.b.d", "[1]", "['my1.2.4']", "[4]", "af", "['micor.a.ee.f']", "ra", "[6]" }
    //        .SelectMany(str => str.StartsWith("[") ? new[] { str } : str.Split('.'))
    //        // > { "a", "b", "d", "[1]", "['my1.2.4']", "[4]", "af", "['micor.a.ee.f']", "ra", "[6]" }
    //        .ToArray();
    //    JToken node = jToken;
    //    for (int i = 0; i < pathParts.Length; i++)
    //    {
    //        var pathPart = pathParts[i];
    //        var partNode = node.SelectToken(pathPart);
    //        //node is null or token with null value
    //        if (partNode == null || partNode.Type == JTokenType.Null)
    //        {
    //            if (i < pathParts.Length - 1)
    //            {
    //                //the next level is array or object
    //                //accept [0], not ['prop']
    //                JToken nextToken = MyRegex().IsMatch(pathParts[i + 1]) ?
    //                    new JArray() : new JObject();
    //                SetToken(node, pathPart, nextToken);
    //            }
    //            else if (i == pathParts.Length - 1)
    //            {
    //                //JToken.FromObject(null) will throw a exception
    //                var jValue = value == null ?
    //                   null : JToken.FromObject(value);
    //                SetToken(node, pathPart, jValue);
    //            }
    //            partNode = node.SelectToken(pathPart);
    //        }
    //        node = partNode;
    //    }
    //    //set new token
    //    static void SetToken(JToken node, string pathPart, JToken jToken)
    //    {
    //        if (node.Type == JTokenType.Object)
    //        {
    //            //get real prop name (convert "['prop']" to "prop")
    //            var name = pathPart.Trim('[', ']', '\'');
    //            ((JObject)node).Add(Regex.Unescape(name), jToken);
    //        }
    //        else if (node.Type == JTokenType.Array)
    //        {
    //            //get real index (convert "[0]" to 0)
    //            var index = int.Parse(pathPart.Trim('[', ']'));
    //            var jArray = (JArray)node;
    //            //if index is bigger than array length, fill the array
    //            while (index >= jArray.Count)
    //                jArray.Add(null);
    //            //set token
    //            jArray[index] = jToken;
    //        }
    //    }
    //}

    [GeneratedRegex("\\[\\d+\\]")]
    private static partial Regex MyRegex();
    [GeneratedRegex("(?=\\[)|(?=\\[\\.)|(?<=])(?>\\.)")]
    private static partial Regex MyRegex1();
}
