using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
namespace StardewValley.Translation.JsonClass;

public static partial class ClassTranslation
{
    public static JsonNode Convert(JsonNode node, string type)
    {
        return type switch
        {
            "StardewValley.GameData.Movies.ConcessionItemData" => Convert<JsonConcessionItemData>(node),
            "StardewValley.GameData.Movies.MovieData" => Convert<JsonMovieData>(node),
            "StardewValley.GameData.Movies.MovieCharacterReaction" => Convert<JsonMovieCharacterReaction>(node),
            _ => throw new ArgumentException(nameof(type))
        };
    }
    
    public static void Apply(JsonNode node, JsonNode translation, string type)
    {
        switch (type)
        {
            case "StardewValley.GameData.Movies.ConcessionItemData":
                Apply<JsonConcessionItemData>(node, translation);
                break;
            case "StardewValley.GameData.Movies.MovieData":
                Apply<JsonMovieData>(node, translation);
                break;
            case "StardewValley.GameData.Movies.MovieCharacterReaction":
                Apply<JsonMovieCharacterReaction>(node, translation);
                break;
            default:
                throw new ArgumentException(nameof(type));
        }
    }
}