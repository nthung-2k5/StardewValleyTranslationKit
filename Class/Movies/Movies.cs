using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StardewValley.Class.Movies;

public partial record Movies(int? ID, int SheetIndex, string Title, string Description, string[] Tags, Scene[] Scenes) : ISVClass, IJsonNode
{
    public static JToken Generate(JToken content)
    {
        var dict = content.ToObject<Dictionary<string, Movies>>();
        return JObject.FromObject(dict.ToDictionary(kv => kv.Key, kv => kv.Value.Convert()));
    }
    public static void Apply(JToken content, JToken mod)
    {
        var dict = mod as JObject;
        foreach (var (k, v) in dict)
        {
            content[k][nameof(Title)].Replace(v[nameof(Title)]);
            content[k][nameof(Description)].Replace(v[nameof(Description)]);

            if (content[k][nameof(Scenes)] is not JArray scenes)
            {
                continue;
            }
            for (int i = 0; i < scenes.Count; i++)
            {
                Scene.Apply(scenes[i], v[nameof(Scenes)][i]);
            }
        }
    }
    public JObject Convert()
    {
        var scenes = Scenes.Select(x => x.Convert()).ToArray();
        if (scenes == null || !scenes.Any())
        {
            scenes = null;
        }    
        var json = new { Title, Description, Scenes = scenes };
        return JObject.FromObject(json);
    }

    //public record Json(string Title, string Description, Scene.Json[] Scenes)
    //{
    //    public bool ShouldSerializeScenes() => Scenes != null && Scenes.Any();
    //}
}
