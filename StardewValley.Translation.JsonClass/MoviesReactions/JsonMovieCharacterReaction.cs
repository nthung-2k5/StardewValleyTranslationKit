using System.Runtime.InteropServices;
using StardewValley.GameData.Movies;

namespace StardewValley.Translation.JsonClass.MoviesReactions;

[JsonClass<MovieCharacterReaction>]
public partial class JsonMovieCharacterReaction
{
    public string NPCName { get; set; }
    public List<JsonMovieReaction>? Reactions { get; set; }

    public override void Read(MovieCharacterReaction data)
    {
        NPCName = data.NPCName;
        Reactions = data.Reactions?.ConvertAll(reaction =>
        {
            var json = new JsonMovieReaction();
            json.Read(reaction);
            return json;
        });
    }

    public override void Write(ref MovieCharacterReaction data)
    {
        data.NPCName = NPCName;

        if (Reactions == null) return;

        var span = CollectionsMarshal.AsSpan(data.Reactions);
        for (int i = 0; i < Reactions.Count; i++)
        {
            Reactions[i].Write(ref span[i]);
        }
    }
}