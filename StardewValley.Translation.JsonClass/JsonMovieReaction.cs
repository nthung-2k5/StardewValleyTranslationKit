using System;
using System.Collections.Generic;
using StardewValley.GameData.Movies;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace StardewValley.Translation.JsonClass
{
    public class JsonMovieReaction : BaseJsonClass<MovieReaction>
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonSpecialResponses? SpecialResponses { get; set; }
        protected override void Read(MovieReaction content)
        {
            if (content.SpecialResponses is not null)
            {
                SpecialResponses = new JsonSpecialResponses { Content = content.SpecialResponses };
            }
        }

        public override void Apply(MovieReaction content)
        {
            SpecialResponses?.Apply(content.SpecialResponses);
        }
    }
}
