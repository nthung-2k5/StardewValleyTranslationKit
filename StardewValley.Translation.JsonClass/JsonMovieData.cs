using System;
using System.Collections.Generic;
using StardewValley.GameData.Movies;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace StardewValley.Translation.JsonClass
{
    public class JsonMovieData : BaseJsonClass<MovieData>
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public List<JsonMovieScene> Scenes { get; set; }
        protected override void Read(MovieData content)
        {
            Title = content.Title;
            Description = content.Description;
            Scenes = content.Scenes.ConvertAll(scene => new JsonMovieScene { Content = scene });
        }

        public override void Apply(MovieData content)
        {
            content.Title = Title;
            content.Description = Description;
            int i = 0;
            content.Scenes.ForEach(scene => Scenes[i++].Apply(scene));
        }
    }
}
