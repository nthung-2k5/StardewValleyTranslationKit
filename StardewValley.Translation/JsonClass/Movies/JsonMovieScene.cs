using StardewValley.GameData.Movies;
using SVTranslation.JsonClass;

namespace StardewValley.JsonClass.Movies;
public record JsonMovieScene(ScriptMessage? Script, string? Text) : IJsonClass<JsonMovieScene, MovieScene>
{
    public static JsonMovieScene Convert(MovieScene data) => new(data.Script, data.Text);

    public void Apply(MovieScene content)
    {
        content.Text = Text;
        Script?.Apply(ref content.Script);
    }
}