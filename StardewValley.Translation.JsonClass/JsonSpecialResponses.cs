using System;
using System.Collections.Generic;
using StardewValley.GameData.Movies;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace StardewValley.Translation.JsonClass
{
    internal class JsonSpecialResponses : BaseJsonClass<SpecialResponses>
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonCharacterResponse? BeforeMovie { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonCharacterResponse? DuringMovie { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonCharacterResponse? AfterMovie { get; set; }
        protected override void Read(SpecialResponses content)
        {
            if (content.BeforeMovie is not null)
            {
                BeforeMovie = new JsonCharacterResponse { Content = content.BeforeMovie };
            }
            if (content.DuringMovie is not null)
            {
                DuringMovie = new JsonCharacterResponse { Content = content.DuringMovie };
            }
            if (content.AfterMovie is not null)
            {
                AfterMovie = new JsonCharacterResponse { Content = content.AfterMovie };
            }
        }

        public override void Apply(SpecialResponses content)
        {
            BeforeMovie?.Apply(content.BeforeMovie);
            DuringMovie?.Apply(content.DuringMovie);
            AfterMovie?.Apply(content.AfterMovie);
        }
    }
}
