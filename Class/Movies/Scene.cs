using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace StardewValley.Class.Movies;
public record Scene(int Image, string Music, string Sound, int MessageDelay, string Script, string Text, bool Shake, string ResponsePoint, string ID) : IJsonNode
{
    public static void Apply(JToken content, JToken mod)
    {
        var modObj = mod as JObject;
        if (modObj.ContainsKey(nameof(Text)))
        {
            content[nameof(Text)].Replace(modObj[nameof(Text)]);
        }
        if (modObj.ContainsKey(nameof(Script)))
        {
            content[nameof(Script)].Replace(content[nameof(Script)].ApplyMessagesDynamic(mod[nameof(Script)]));
        }
    }

    public JObject Convert() => JObject.FromObject(new { Text, Script = Script.GetMessagesDynamic() });

    //public record Json(string Text, string[] Script)
    //{
    //    public bool ShouldSerializeScript() => Script != null && Script.Any();
    //}
}