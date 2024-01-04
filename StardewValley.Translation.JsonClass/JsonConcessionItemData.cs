using System;
using System.Collections.Generic;
using StardewValley.GameData.Movies;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace StardewValley.Translation.JsonClass
{
    public class JsonConcessionItemData : BaseJsonClass<ConcessionItemData>
    {
        public string DisplayName { get; set; }

        public string Description { get; set; }
        protected override void Read(ConcessionItemData content)
        {
            DisplayName = content.DisplayName;
            Description = content.Description;
        }

        public override void Apply(ConcessionItemData content)
        {
            content.DisplayName = DisplayName;
            content.Description = Description;
        }
    }
}
