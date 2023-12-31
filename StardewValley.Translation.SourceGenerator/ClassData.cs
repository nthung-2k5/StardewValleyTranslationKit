namespace StardewValley.Translation.SourceGenerator;

public class ClassData(string BaseClass, string[] Mapping, string[] ScriptMessage)
{
    public string BaseClass { get; } = BaseClass;
    public string[] Mapping { get; } = Mapping;
    public string[] ScriptMessage { get; } = ScriptMessage;

    public void Deconstruct(out string BaseClass, out string[] Mapping, out string[] ScriptMessage)
    {
        BaseClass = this.BaseClass;
        Mapping = this.Mapping;
        ScriptMessage = this.ScriptMessage;
    }
}