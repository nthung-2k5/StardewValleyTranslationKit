using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Json.Path;

namespace StardewValley.Translation.Helper;
public static partial class JsonHelper
{
    public static bool IsClass(JsonArray readers, out string? value)
    {
        foreach (var reader in readers.Cast<JsonObject>())
        {
            string type = reader["type"]!.GetValue<string>();

            if (!type.Contains("ReflectiveReader")) continue;

            var res = TypeParsingRegex.Match(type);
            var genericArgCount = res.Groups["GenericArgCount"];
            var genericArgs = res.Groups["Arg"];

            if (genericArgCount?.Success != true) // Not a generic, lets short-circuit
            {
                continue;
            }

            value = TypeDetailsRegex.Match(genericArgs.Captures[0].Value).Groups["TypeName"].Value;
            return true;
        }

        value = null;
        return false;
    }

    private static readonly Regex TypeParsingRegex = TypeParse();

    public static JsonNode StubContainer(this JsonNode node) =>
        node.GetValueKind() == JsonValueKind.Object ? new JsonObject() : new JsonArray();
    [GeneratedRegex(
        @"^(?<TypeName>[\w\.]+(?:`(?<GenericArgCount>\d+))?)(?:\[(?:(?<Arg>\[(?>(?<c>\[)|(?<-c>\])|[^[\]]+)+\](?(c)(?!))),?\s*)+\])?",
        RegexOptions.Compiled)]
    private static partial Regex TypeParse();

    private static readonly Regex TypeDetailsRegex = TypeDetail();

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
        
        var node = jToken;
        for (int i = 0; i < pathParts.Length; i++)
        {
            string pathPart = pathParts[i];
            var partNode = node!.SelectToken(pathPart);
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
                    var jValue = JsonSerializer.SerializeToNode(value);
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
                //get real prop name (convert "['prop']" to "prop")
                string name = pathPart.Trim('[', ']', '\'');
                node.AsObject().Add(Regex.Unescape(name), token);
            }
            else if (node.GetValueKind() == JsonValueKind.Array)
            {
                //get real index (convert "[0]" to 0)
                int index = int.Parse(pathPart.Trim('[', ']'));
                var arr = node.AsArray();
                //if index is bigger than array length, fill the array
                while (index >= arr.Count)
                    arr.Add(null);
                //set token
                arr[index] = token;
            }
        }
    }

    public static JsonNode? SelectToken(this JsonNode node, string path)
    {
        var jPath = JsonPath.Parse('$' + path.TrimStart('$'));
        var result = jPath.Evaluate(node);
        return result.Matches?.FirstOrDefault()?.Value;
    }
    
    public static JsonNode? SelectToken(this JsonNode node, JsonPath path)
    {
        var result = path.Evaluate(node);
        return result.Matches?.FirstOrDefault()?.Value;
    }
    
    [GeneratedRegex(@"\[\d+\]")]
    private static partial Regex MyRegex();
    [GeneratedRegex(@"(?=\[)|(?=\[\.)|(?<=])(?>\.)")]
    private static partial Regex MyRegex1();
}