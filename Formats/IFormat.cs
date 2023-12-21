using System.Text.Json;

namespace StardewValley.Formats;
public interface IFormat
{
    public string FormatPath { get; set; }
    public JsonElement Content { get; set; }
    public abstract void Export();
    public abstract void Import();
}
