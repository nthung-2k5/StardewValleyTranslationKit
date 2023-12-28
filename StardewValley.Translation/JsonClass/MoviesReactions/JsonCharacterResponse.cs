using StardewValley.GameData.Movies;

namespace StardewValley.Translation.JsonClass.MoviesReactions;

public record JsonCharacterResponse(ScriptMessage? Script, string? Text) : IJsonClass<JsonCharacterResponse, CharacterResponse>
{
    public static JsonCharacterResponse Convert(CharacterResponse data) => new(data.Script, data.Text);

    public void Apply(CharacterResponse content)
    {
        Script?.Apply(ref content.Script);
        content.Text = Text;
    }
}