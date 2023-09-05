using Newtonsoft.Json.Linq;

namespace StardewValley.Class.MoviesReactions;

public record MoviesReactions(string NPCName, Reaction[] Reactions) : ISVClass
{
    public static JToken Generate(JToken content)
    {
        var list = content.ToObject<List<MoviesReactions>>();
        static JObject ToJson(MoviesReactions react) => JObject.FromObject(react.Reactions.Where(rep => rep.SpecialResponses is not null).ToDictionary(reaction => reaction.ID, reaction => reaction.SpecialResponses.Convert()));
        return JObject.FromObject(list.ToDictionary(react => react.NPCName, ToJson));
    }
    public static void Apply(JToken content, JToken mod)
    {

    }

    
}