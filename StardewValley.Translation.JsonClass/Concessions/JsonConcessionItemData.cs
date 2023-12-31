using StardewValley.GameData.Movies;

namespace StardewValley.Translation.JsonClass.Concessions;

[JsonClass<ConcessionItemData>]
public partial class JsonConcessionItemData
{
    public string DisplayName { get; set; } = null!;
    public string Description { get; set; } = null!;

    public override void Read(ConcessionItemData data)
    {
        DisplayName = data.DisplayName;
        Description = data.Description;
    }

    public override void Write(ref ConcessionItemData data)
    {
        data.DisplayName = DisplayName;
        data.Description = Description;
    }
}
