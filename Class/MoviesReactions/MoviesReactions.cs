using Newtonsoft.Json.Linq;
using System.Linq;

namespace StardewValley.Class.MoviesReactions;

public record MoviesReactions(string NPCName, Reaction[] Reactions) : ISVClass
{
    public static JToken Generate(JToken content)
    {
        var list = content.ToObject<List<MoviesReactions>>();
        static JObject ToJson(MoviesReactions react)
        {
            return JObject.FromObject(react.Reactions.Where(rep => rep.SpecialResponses is not null).ToDictionary(reaction => reaction.ID, reaction => reaction.SpecialResponses.Convert()));
        }
        return JObject.FromObject(list.ToDictionary(react => react.NPCName, ToJson));
    }
    public static void Apply(JToken content, JToken mod)
    {
        var obj = mod as JObject;
        foreach (var reactions in ((JArray)content).Where(reactions => obj.ContainsKey(reactions["NPCName"].Value<string>())))
        {

            var reacts = reactions["Reactions"] as JArray;
            var modReacts = obj[reactions["NPCName"].Value<string>()] as JObject;

            foreach (var react in reacts.Where(react => modReacts.ContainsKey(react["ID"].Value<string>())))
            {
                SpecialResponses.Apply(react, modReacts[react["ID"].Value<string>()]);
            }
        }
    }

    
}