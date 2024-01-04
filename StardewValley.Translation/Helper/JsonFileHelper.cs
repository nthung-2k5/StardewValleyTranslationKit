using System.Text.Json.Nodes;

namespace StardewValley.Translation.Helper;

public static class JsonFileHelper
{
    public static JsonObject GetLanguageData(string folder, string basename, string language = "") => GetLanguageData(GetLanguageFile(folder, basename, language));

    private static JsonObject GetLanguageData(string path)
    {
        if (!File.Exists(path))
        {
            return [];
        }

        using FileStream file = File.OpenRead(path);

        return JsonNode.Parse(file)!.AsObject();
    }

    public static string GetLanguageFile(string folder, string basename, string language = "") =>
        Path.ChangeExtension(Path.Combine(folder, basename), language + ".json");

    public static IEnumerable<string> GetBaseLanguageFileNames(string folder, string language)
    {
        return Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories)
                        .Select(subfolder => PathHelper.RemoveAllExtensions(Path.GetRelativePath(folder, subfolder)))
                        .GroupBy(sub => sub).Where(sub => File.Exists(GetLanguageFile(folder, sub.Key, language)))
                        .Select(group => group.Key);
    }
}
