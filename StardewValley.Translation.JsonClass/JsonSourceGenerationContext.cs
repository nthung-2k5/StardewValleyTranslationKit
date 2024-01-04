using StardewValley.GameData.Movies;
using System.Text.Json.Serialization;

namespace StardewValley.Translation.JsonClass;

[JsonSourceGenerationOptions(IncludeFields = true)]
[JsonSerializable(typeof(JsonConcessionItemData))]
[JsonSerializable(typeof(JsonMovieData))]
[JsonSerializable(typeof(JsonMovieScene))]
[JsonSerializable(typeof(JsonCharacterResponse))]
[JsonSerializable(typeof(JsonMovieCharacterReaction))]
[JsonSerializable(typeof(JsonMovieReaction))]
[JsonSerializable(typeof(JsonSpecialResponses))]
[JsonSerializable(typeof(ConcessionItemData))]
[JsonSerializable(typeof(MovieData))]
[JsonSerializable(typeof(MovieScene))]
[JsonSerializable(typeof(CharacterResponse))]
[JsonSerializable(typeof(MovieCharacterReaction))]
[JsonSerializable(typeof(MovieReaction))]
[JsonSerializable(typeof(SpecialResponses))]
public partial class JsonSourceGenerationContext: JsonSerializerContext;