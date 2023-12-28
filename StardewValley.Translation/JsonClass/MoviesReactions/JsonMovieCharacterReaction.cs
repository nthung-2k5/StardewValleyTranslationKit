using StardewValley.GameData.Movies;
using StardewValley.JsonClass.Movies;
using System.Linq;
using System.Runtime.InteropServices;

namespace StardewValley.JsonClass.MoviesReactions;

public record JsonMovieCharacterReaction(string NPCName, List<JsonMovieReaction>? Reactions) : IJsonClass<JsonMovieCharacterReaction, MovieCharacterReaction>
{
    public static JsonMovieCharacterReaction Convert(MovieCharacterReaction data) => new(data.NPCName, data.Reactions?.ConvertAll(JsonMovieReaction.Convert));

    public void Apply(MovieCharacterReaction content)
    {
        content.NPCName = NPCName;

        if (Reactions == null) return;

        for (int i = 0; i < Reactions.Count; i++)
        {
            Reactions[i].Apply(content.Reactions[i]);
        }
    }
}