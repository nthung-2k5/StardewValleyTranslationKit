using System.Text.Json.Nodes;
using StardewValley.Translation.Helper;
using static StardewValley.Translation.Helper.JsonFileHelper;

namespace StardewValley.Translation.Process;

public interface IProcess
{
    void Process();
}
public abstract class BaseProcess<T>(string folder, string? exportFolder): IProcess where T : ProcessContext
{
    protected const string Language = "es-ES";

    protected string ExportFolder => exportFolder ?? folder;
    protected string Folder => folder;

    public virtual void Process()
    {
        foreach (string file in GetBaseLanguageFileNames(folder, Language))
        {
            (T context, JsonNode? header) = ProcessFile(file);
            OnFileProcessed((file, ProcessType(context, header)));
        }

        OnAllFileProcessed();
    }

    protected abstract (T, JsonNode?) ProcessFile(string file);

    private JsonNode ProcessType(T context, JsonNode? header) =>
        JsonHelper.IsClass(header!.AsArray(), out string? type) && type is not null
            ? ProcessClass(context, type)
            : ProcessPrimitive(context);

    protected abstract JsonNode ProcessPrimitive(T context);
    protected abstract JsonNode ProcessClass(T context, string type);

    public event EventHandler<(string file, JsonNode content)>? FileProcessed;

    public event EventHandler? AllFileProcessed;

    protected virtual void OnFileProcessed((string file, JsonNode content) e)
    {
        FileProcessed?.Invoke(this, e);
    }

    protected virtual void OnAllFileProcessed()
    {
        AllFileProcessed?.Invoke(this, EventArgs.Empty);
    }
}
