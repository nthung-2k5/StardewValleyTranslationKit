using System.Text.Json.Serialization;
using StardewValley.GameData.Movies;

namespace StardewValley.Translation.JsonClass.MoviesReactions;

[JsonClass<SpecialResponses>]
public partial class JsonSpecialResponses
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonCharacterResponse? BeforeMovie { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonCharacterResponse? DuringMovie { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonCharacterResponse? AfterMovie { get; set; }
    public override void Read(SpecialResponses data)
    {
        if (data.BeforeMovie is not null)
        {
            BeforeMovie = new JsonCharacterResponse();
            BeforeMovie.Read(data.BeforeMovie);
        }
        if (data.DuringMovie is not null)
        {
            DuringMovie = new JsonCharacterResponse();
            DuringMovie.Read(data.DuringMovie);
        }
        if (data.AfterMovie is not null)
        {
            AfterMovie = new JsonCharacterResponse();
            AfterMovie.Read(data.AfterMovie);
        }
    }

    public override void Write(ref SpecialResponses data)
    {
        BeforeMovie?.Write(ref data.BeforeMovie);
        DuringMovie?.Write(ref data.DuringMovie);
        AfterMovie?.Write(ref data.AfterMovie);
    }
}

