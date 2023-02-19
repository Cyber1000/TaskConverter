using System.Text.Json.Serialization;

namespace Converter.Core.GTD.Model;

public class TaskInfoContextEntry : TaskInfoEntryBaseWithParent
{
    [JsonPropertyOrder(-840)]
    public int Children { get; set; }
}
