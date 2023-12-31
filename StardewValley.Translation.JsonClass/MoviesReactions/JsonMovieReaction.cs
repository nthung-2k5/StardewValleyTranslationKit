using System.Text.Json.Serialization;
using StardewValley.GameData.Movies;

namespace StardewValley.Translation.JsonClass.MoviesReactions;

[JsonClass<MovieReaction>]
public partial class JsonMovieReaction
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonSpecialResponses? SpecialResponses { get; set; }

    public override void Read(MovieReaction data)
    {
        if (data.SpecialResponses is not null)
        {
            SpecialResponses = new JsonSpecialResponses();
            SpecialResponses.Read(data.SpecialResponses);
        }
    }

    public override void Write(ref MovieReaction data)
    {
        SpecialResponses?.Write(ref data.SpecialResponses);
    }
}
