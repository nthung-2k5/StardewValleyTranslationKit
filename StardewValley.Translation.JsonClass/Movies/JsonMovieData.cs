using System.Runtime.InteropServices;
using StardewValley.GameData.Movies;

namespace StardewValley.Translation.JsonClass.Movies;
[JsonClass<MovieData>]
public partial class JsonMovieData
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<JsonMovieScene> Scenes { get; set; } = null!;
    
    public override void Read(MovieData data)
    {
        Title = data.Title;
        Description = data.Description;
        Scenes = data.Scenes.ConvertAll(scene => 
        {
            var json = new JsonMovieScene();
            json.Read(scene);
            return json;
        });
    }

    public override void Write(ref MovieData data)
    {
        data.Title = Title;
        data.Description = Description;

        var span = CollectionsMarshal.AsSpan(data.Scenes);
        for (int i = 0; i < Scenes.Count; i++)
        {
            Scenes[i].Write(ref span[i]);
        }
    }
}
