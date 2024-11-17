using System.Text.Json.Serialization;

namespace TaskConverter.Plugin.GTD.Model;

public class GTDTaskNoteModel : GTDExtendedModel
{
    [JsonPropertyOrder(-850)]
    [JsonPropertyName("TASK_ID")]
    public int TaskID { get; set; }
}
