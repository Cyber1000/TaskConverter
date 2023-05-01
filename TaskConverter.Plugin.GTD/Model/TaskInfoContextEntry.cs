using System.Text.Json.Serialization;

namespace TaskConverter.Plugin.GTD.Model;

public class TaskInfoContextEntry : TaskInfoEntryBaseWithParent
{
    [JsonPropertyOrder(-840)]
    public int Children { get; set; }
}
