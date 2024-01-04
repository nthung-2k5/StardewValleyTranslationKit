using System;
using System.Collections.Generic;
using StardewValley.GameData.Movies;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace StardewValley.Translation.JsonClass
{
    internal class JsonMovieScene : BaseJsonClass<MovieScene>
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Text { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Script? Script { get; set; }
        protected override void Read(MovieScene content)
        {
            Text = content.Text;
            Script = Script.From(content.Script);
        }

        public override void Apply(MovieScene content)
        {
            content.Text = Text;
            Script?.Apply(ref content.Script);
        }
    }
}
