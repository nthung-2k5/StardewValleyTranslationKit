using StardewValley.GameData.Movies;
namespace StardewValley.JsonClass.MoviesReactions;
public record JsonSpecialResponses(JsonCharacterResponse? BeforeMovie, JsonCharacterResponse? DuringMovie, JsonCharacterResponse? AfterMovie) : IJsonClass<JsonSpecialResponses, SpecialResponses>
{
    public static JsonSpecialResponses Convert(SpecialResponses data) => new(
        data.BeforeMovie?.Convert<CharacterResponse, JsonCharacterResponse>(),
        data.DuringMovie?.Convert<CharacterResponse, JsonCharacterResponse>(),
        data.AfterMovie?.Convert<CharacterResponse, JsonCharacterResponse>());

    public void Apply(SpecialResponses content)
    {
        BeforeMovie?.Apply(content.BeforeMovie);
        DuringMovie?.Apply( content.DuringMovie);
        AfterMovie?.Apply(content.AfterMovie);
    }
}

