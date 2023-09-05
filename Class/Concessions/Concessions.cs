using Newtonsoft.Json.Linq;
using StardewValley.Class.Movies;

namespace StardewValley.Class.Concessions;

public record Concessions(int ID, string Name, string DisplayName, string Description, int Price, string[] ItemTags) : ISVClass, IJsonNode
{
    //public record Json(string DisplayName, string Description);

    public static JToken Generate(JToken content)
    {
        var list = content.ToObject<Concessions[]>();
        return new JArray(list.Select(obj => obj.Convert()));
    }

    public static void Apply(JToken content, JToken mod)
    {
        var arr = content as JArray;
        for (int i = 0; i < arr.Count; i++)
        {
            content[i][nameof(DisplayName)].Replace(mod[i][nameof(DisplayName)]);
            content[i][nameof(Description)].Replace(mod[i][nameof(Description)]);
        }
    }
    public JObject Convert() => JObject.FromObject(new { DisplayName, Description });
}
