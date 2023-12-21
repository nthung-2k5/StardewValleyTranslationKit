using Newtonsoft.Json.Linq;

namespace StardewValley.Class;
internal interface IJsonNode
{
    JObject Convert();
    public static abstract void Apply(JToken content, JToken mod);
}
