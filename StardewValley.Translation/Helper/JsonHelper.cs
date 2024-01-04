using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Json.Path;

namespace StardewValley.Translation.Helper;

public static partial class JsonHelper
{
    private static readonly Regex TypeParsingRegex = TypeParse();

    private static readonly Regex TypeDetailsRegex = TypeDetail();

    public static bool IsClass(JsonArray readers, out string? value)
    {
        foreach (JsonObject? reader in readers.Cast<JsonObject>())
        {
            string type = reader["type"]!.GetValue<string>();

            if (!type.Contains("ReflectiveReader"))
            {
                continue;
            }

            Match res = TypeParsingRegex.Match(type);
            Group genericArgCount = res.Groups["GenericArgCount"];
            Group genericArgs = res.Groups["Arg"];

            if (genericArgCount.Success != true) // Not a generic, lets short-circuit
            {
                continue;
            }

            value = TypeDetailsRegex.Match(genericArgs.Captures[0].Value).Groups["TypeName"].Value;

            return true;
        }

        value = null;

        return false;
    }

    public static JsonNode StubContainer(this JsonNode node) => node.GetValueKind() == JsonValueKind.Object ? new JsonObject() : new JsonArray();

    [GeneratedRegex(@"^(?<TypeName>[\w\.]+(?:`(?<GenericArgCount>\d+))?)(?:\[(?:(?<Arg>\[(?>(?<c>\[)|(?<-c>\])|[^[\]]+)+\](?(c)(?!))),?\s*)+\])?", RegexOptions.Compiled)]
    private static partial Regex TypeParse();

    [GeneratedRegex(@"^\[(?<TypeName>.+),(?:.+,){3}.+\]$", RegexOptions.Compiled)]
    private static partial Regex TypeDetail();

    public static void AddTokenByPath(this JsonNode jToken, string path, object value)
    {
        // "a.b.d[1]['my1.2.4'][4].af['micor.a.ee.f'].ra[6]"
        string[] pathParts = MyRegex1().Split(path)
                                       // > { "a.b.d", "[1]", "['my1.2.4']", "[4]", "af", "['micor.a.ee.f']", "ra", "[6]" }
                                       .SelectMany(str => str.StartsWith('[') ? [str] : str.Split('.'))
                                       // > { "a", "b", "d", "[1]", "['my1.2.4']", "[4]", "af", "['micor.a.ee.f']", "ra", "[6]" }
                                       .ToArray();

        JsonNode? node = jToken;

        for (int i = 0; i < pathParts.Length; i++)
        {
            string pathPart = pathParts[i];
            JsonNode? partNode = node!.SelectToken(pathPart);

            //node is null or token with null value
            if (partNode == null || partNode.GetValueKind() == JsonValueKind.Null)
            {
                if (i < pathParts.Length - 1)
                {
                    //the next level is array or object
                    //accept [0], not ['prop']
                    JsonNode nextToken = MyRegex().IsMatch(pathParts[i + 1]) ? new JsonArray() : new JsonObject();
                    SetToken(node!, pathPart, nextToken);
                }
                else if (i == pathParts.Length - 1)
                {
                    JsonNode? jValue = JsonSerializer.SerializeToNode(value);
                    SetToken(node!, pathPart, jValue);
                }

                partNode = node!.SelectToken(pathPart);
            }

            node = partNode;
        }

        return;

        //set new token
        static void SetToken(JsonNode node, string pathPart, JsonNode? token)
        {
            if (node.GetValueKind() == JsonValueKind.Object)
            {
                //get real prop name (convert "['prop']" to "prop") and unescape it (compatibility to Newtonsoft.Json)
                node.AsObject().Add(Regex.Unescape(pathPart[2..^2]), token);
            }
            else if (node.GetValueKind() == JsonValueKind.Array)
            {
                //get real index (convert "[0]" to 0)
                int index = int.Parse(pathPart.AsSpan()[1..^1]);
                JsonArray arr = node.AsArray();

                //if index is bigger than array length, fill the array
                while (index >= arr.Count)
                {
                    arr.Add(null);
                }

                //set token
                arr[index] = token;
            }
        }
    }

    public static JsonNode? SelectToken(this JsonNode node, string path)
    {
        JsonPath jPath = JsonPath.Parse('$' + path.TrimStart('$'));
        PathResult result = jPath.Evaluate(node);

        return result.Matches?.FirstOrDefault()?.Value;
    }

    public static JsonNode? SelectToken(this JsonNode node, JsonPath path)
    {
        PathResult result = path.Evaluate(node);

        return result.Matches?.FirstOrDefault()?.Value;
    }

    [GeneratedRegex(@"\[\d+\]")]
    private static partial Regex MyRegex();

    [GeneratedRegex(@"(?=\[)|(?=\[\.)|(?<=])(?>\.)")]
    private static partial Regex MyRegex1();
}
