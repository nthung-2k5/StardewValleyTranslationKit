using System.Text.Json.Serialization;
using StardewValley.GameData.Movies;

namespace StardewValley.Translation.JsonClass.Movies;

[JsonClass<MovieScene>]
public partial class JsonMovieScene
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Script? Script { get; set; }
    public string? Text { get; set; }

    public override void Read(MovieScene data)
    {
        Script = Script.From(data.Script);
        Text = data.Text;
    }

    public override void Write(ref MovieScene data)
    {
        data.Text = Text;
        Script?.Apply(ref data.Script);
    }
}