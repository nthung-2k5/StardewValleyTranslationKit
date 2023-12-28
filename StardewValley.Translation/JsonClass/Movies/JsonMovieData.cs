using StardewValley.GameData.Movies;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace StardewValley.JsonClass.Movies;

public partial record JsonMovieData : IJsonClass<JsonMovieData, MovieData>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public List<JsonMovieScene> Scenes { get; set; }
    public void Apply(ref MovieData content)
    {
        content.Title = Title;
        content.Description = Description;

        for (int i = 0; i < Scenes.Count; i++)
        {
            Scenes[i].Apply(content.Scenes[i]);
        }
    }

    public static JsonMovieData Convert(MovieData data) => new() { data.Title, data.Description, data.Scenes.ConvertAll(JsonMovieScene.Convert) };
}
