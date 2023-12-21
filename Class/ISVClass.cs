using Newtonsoft.Json.Linq;

namespace StardewValley.Class;

public interface ISVClass
{
    public static abstract JToken Generate(JToken content);
    public static abstract void Apply(JToken content, JToken mod);
}
public enum ClassEnum
{
    Concessions,
    Movies,
    MoviesReactions
}