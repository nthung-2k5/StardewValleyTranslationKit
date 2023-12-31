using System.Text;
using System.Text.Json.Serialization;

namespace StardewValley.Translation.JsonClass;

public class Script
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Messages { get; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<SpeakScript>? CharacterSpeaks { get; }

    private Script(string script)
    {
        foreach (string split in script.Split('/'))
        {
            if (split.StartsWith("message"))
            {
                Messages ??= [];
                Messages.Add(split.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim('\\', '"'));
            }
            else if (split.StartsWith("speak"))
            {
                CharacterSpeaks ??= [];
                string[] splits = split.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);
                (string character, string text) = (splits[1], splits[2].Trim('\\', '"'));
                CharacterSpeaks.Add(new SpeakScript(character, text));
            }
        }
    }
    public void Apply(ref string script)
    {
        string[] split = script.Split('/');
        int messageCount = 0;
        int speakCount = 0;
        
        for (int i = 0; i < split.Length; i++)
        {
            if (split[i].StartsWith("message"))
            {
                split[i] = $"""
                            message \"{Messages![messageCount]}\"
                            """;
                messageCount++;
            }
            else if (split[i].StartsWith("speak"))
            {
                split[i] = $"speak {CharacterSpeaks![speakCount]}";
                speakCount++;
            }
        }

        script = string.Join('/', split);
    }
    
    public static Script? From(string script)
    {
        var scriptClass = new Script(script);
        return scriptClass.Messages is not null || scriptClass.CharacterSpeaks is not null ? scriptClass : null;
    }

    public static bool HasText(string script) => script.Contains("/message") || script.Contains("/speak");
}
public record SpeakScript(string Character, string Text)
{
    public override string ToString()
    {
        StringBuilder sb = new($"{Character} ");
        if (Text.StartsWith('{'))
        {
            sb.Append(@"\""").Append(Text).Append(@"\""");
        }
        else
        {
            sb.Append(Text);
        }

        return sb.ToString();
    }
}