using System;
using System.Collections.Generic;
using StardewValley.GameData.Movies;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace StardewValley.Translation.JsonClass
{
    internal class JsonMovieCharacterReaction : BaseJsonClass<MovieCharacterReaction>
    {
        public string NPCName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<JsonMovieReaction>? Reactions { get; set; }
        protected override void Read(MovieCharacterReaction content)
        {
            NPCName = content.NPCName;
            Reactions = content.Reactions?.ConvertAll(reaction => new JsonMovieReaction { Content = reaction });
        }

        public override void Apply(MovieCharacterReaction content)
        {
            content.NPCName = NPCName;
            int i = 0;
            content.Reactions?.ForEach(reaction => Reactions[i++].Apply(reaction));
        }
    }
}
