using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StardewValley.Class.MoviesReactions;
public record SpecialResponses(SpecialResponse BeforeMovie, SpecialResponse DuringMovie, SpecialResponse AfterMovie) : IJsonNode
{
    public static void Apply(JToken content, JToken mod)
    {
        if (mod["BeforeMovie"] != null)
        {
            SpecialResponse.Apply(content["SpecialResponses"]["BeforeMovie"], mod["BeforeMovie"]);
        }
        if (mod["DuringMovie"] != null)
        {
            SpecialResponse.Apply(content["SpecialResponses"]["DuringMovie"], mod["DuringMovie"]);
        }
        if (mod["AfterMovie"] != null)
        {
            SpecialResponse.Apply(content["SpecialResponses"]["AfterMovie"], mod["AfterMovie"]);
        }
    }

    public JObject Convert()
    => JObject.FromObject(new { BeforeMovie = BeforeMovie?.Convert(), DuringMovie = DuringMovie?.Convert(), AfterMovie = AfterMovie?.Convert() });

    //[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    //public record Json(SpecialResponse.Json BeforeMovie, SpecialResponse.Json DuringMovie, SpecialResponse.Json AfterMovie);
}

