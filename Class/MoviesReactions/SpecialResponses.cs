using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StardewValley.Class.MoviesReactions;
public record SpecialResponses(SpecialResponse BeforeMovie, SpecialResponse DuringMovie, SpecialResponse AfterMovie) : IJsonNode
{
    public static void Apply(JToken content, JToken mod)
    {
        throw new NotImplementedException();
    }

    public JObject Convert()
    => JObject.FromObject(new { BeforeMovie = BeforeMovie?.Convert(), DuringMovie = DuringMovie?.Convert(), AfterMovie = AfterMovie?.Convert() });

    //[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    //public record Json(SpecialResponse.Json BeforeMovie, SpecialResponse.Json DuringMovie, SpecialResponse.Json AfterMovie);
}

