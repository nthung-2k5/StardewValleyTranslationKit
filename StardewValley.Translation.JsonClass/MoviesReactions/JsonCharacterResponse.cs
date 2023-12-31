using System.Text.Json.Serialization;
using StardewValley.GameData.Movies;

namespace StardewValley.Translation.JsonClass.MoviesReactions;
[JsonClass<CharacterResponse>]
public partial class JsonCharacterResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Script? Script { get; set; }
    public string? Text { get; set; }

    public override void Read(CharacterResponse data)
    {
        Script = Script.From(data.Script);
        Text = data.Text;
    }

    public override void Write(ref CharacterResponse data)
    {
        Script?.Apply(ref data.Script);
        data.Text = Text;
    }
}