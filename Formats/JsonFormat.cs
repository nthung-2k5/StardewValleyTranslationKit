using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewValley.Formats;

namespace SVTranslation.Formats;
internal class JsonFormat : BaseFormat, IDisposable
{
    private bool disposedValue;

    public JsonFormat(string path, JObject json, string name) : base(path, json) { FileStream = File.Create(name); }
    public JsonFormat(string path, JObject json, FileStream stream) : base(path, json) { FileStream = stream; }
    public JsonFormat(string name) : this(File.Create(name)) { }
    public JsonFormat(FileStream stream): base(null, new()) { FileStream = stream; }
    public override void Export()
    {
        using StreamWriter sw = new(FileStream);
        using JsonTextWriter jw = new(sw);

        jw.Formatting = Formatting.Indented;
        jw.IndentChar = ' ';
        jw.Indentation = 4;

        Content.WriteTo(jw);
    }
    public override void Import()
    {
        using StreamReader reader = new(FileStream);
        Content = JObject.Parse(reader.ReadToEnd());
    }

    public FileStream FileStream { get; set; }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                FileStream?.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
