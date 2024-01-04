using StardewValley.GameData.Movies;
using System.Text.Json.Serialization;

namespace StardewValley.Translation.JsonClass;

[JsonSourceGenerationOptions(IncludeFields = true)]
[JsonSerializable(typeof(JsonCharacterResponse))]
[JsonSerializable(typeof(JsonConcessionItemData))]
[JsonSerializable(typeof(JsonMovieCharacterReaction))]
[JsonSerializable(typeof(JsonMovieData))]
[JsonSerializable(typeof(JsonMovieReaction))]
[JsonSerializable(typeof(JsonMovieScene))]
[JsonSerializable(typeof(JsonSpecialResponses))]
[JsonSerializable(typeof(CharacterResponse))]
[JsonSerializable(typeof(ConcessionItemData))]
[JsonSerializable(typeof(MovieCharacterReaction))]
[JsonSerializable(typeof(MovieData))]
[JsonSerializable(typeof(MovieReaction))]
[JsonSerializable(typeof(MovieScene))]
[JsonSerializable(typeof(SpecialResponses))]
internal partial class JsonSourceGenerationContext: JsonSerializerContext;