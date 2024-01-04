using System;
using System.Collections.Generic;
using StardewValley.GameData.Movies;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace StardewValley.Translation.JsonClass
{
    internal class JsonCharacterResponse : BaseJsonClass<CharacterResponse>
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Text { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Script? Script { get; set; }
        protected override void Read(CharacterResponse content)
        {
            Text = content.Text;
            Script = Script.From(content.Script);
        }

        public override void Apply(CharacterResponse content)
        {
            content.Text = Text;
            Script?.Apply(ref content.Script);
        }
    }
}
