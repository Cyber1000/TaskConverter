using System.Text.Json.Serialization;

namespace Converter.Plugin.GTD.Model;

public abstract class TaskInfoExtendedEntry : TaskInfoEntryBase
{
    [JsonPropertyOrder(-500)]
    public int Color { get; set; }

    [JsonPropertyOrder(-400)]
    public bool Visible { get; set; }
}
