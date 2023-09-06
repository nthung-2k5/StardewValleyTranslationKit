using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StardewValley.Class.MoviesReactions;
public record SpecialResponse(string ResponsePoint, string Script, string Text) : IJsonNode
{
    public static void Apply(JToken content, JToken mod)
    {
        var obj = content as JObject;
        if (obj.ContainsKey("Text"))
        {
            content["Text"].Replace(obj["Text"]);
        }
        if (obj.ContainsKey("Script"))
        {
            content["Text"].Replace(content["Text"].ApplyMessagesDynamic(obj["Script"]));
        }    
    }

    public JObject Convert() => JObject.FromObject(new { Script = Script.GetMessagesDynamic(), Text });


    //[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    //public record Json(string[] Script, string Text)
    //{
    //    public bool ShouldSerializeScript() => Script != null && Script.Any();
    //}
}