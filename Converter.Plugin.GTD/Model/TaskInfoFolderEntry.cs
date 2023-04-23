using System.Text.Json.Serialization;

namespace Converter.Plugin.GTD.Model;

public class TaskInfoFolderEntry : TaskInfoEntryBaseWithParent
{
    [JsonPropertyOrder(-840)]
    public int Children { get; set; }
    public int Ordinal { get; set; }
}
