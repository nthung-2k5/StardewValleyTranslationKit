using System.Text.Json;

namespace StardewValley.Translation.Process.Import;
public interface IImportSingle
{
    void Read(FileStream fs, JsonElement json);
}
public interface IImportMulti
{
    void Read();
}
