using System.Text.Json.Serialization;

namespace Converter.Plugin.GTD.Model;

public abstract class TaskInfoEntryBaseWithParent : TaskInfoExtendedEntry
{
    [JsonPropertyOrder(-850)]
    public int Parent { get; set; }
}
