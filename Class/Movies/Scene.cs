using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace StardewValley.Class.Movies;
public record Scene(int Image, string Music, string Sound, int MessageDelay, string Script, string Text, bool Shake, string ResponsePoint, string ID) : IJsonNode
{
    public static void Apply(JToken content, JToken mod)
    {
        var arr = content as JArray;
        for (int i = 0; i < arr.Count; i++)
        {
            var obj = content[i] as JObject;
            if (obj.ContainsKey(nameof(Text)))
            {
                obj[nameof(Text)].Replace(mod[i][nameof(Text)]);
            }
            if (obj.ContainsKey(nameof(Script)))
            {
                obj[nameof(Script)].Replace(obj[nameof(Script)].ToObject<string>().ApplyMessages(mod[i][nameof(Script)].Cast<string>().ToArray()));
            }
        }
    }

    public JObject Convert() => JObject.FromObject(new { Text, Script = Script.GetMessagesDynamic() });

    //public record Json(string Text, string[] Script)
    //{
    //    public bool ShouldSerializeScript() => Script != null && Script.Any();
    //}
}