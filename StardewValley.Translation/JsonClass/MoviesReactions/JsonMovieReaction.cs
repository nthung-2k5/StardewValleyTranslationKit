using StardewValley.GameData.Movies;

namespace StardewValley.JsonClass.MoviesReactions;
public record JsonMovieReaction(JsonSpecialResponses? SpecialResponses) : IJsonClass<JsonMovieReaction, MovieReaction>
{
    public static JsonMovieReaction Convert(MovieReaction data) => new(JsonSpecialResponses.Convert(data.SpecialResponses));

    public void Apply(MovieReaction content) => SpecialResponses?.Apply(content.SpecialResponses);
}
