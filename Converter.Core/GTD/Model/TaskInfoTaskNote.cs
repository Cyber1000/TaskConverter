using System.Text.Json.Serialization;

namespace Converter.Core.GTD.Model
{
    public class TaskInfoTaskNote : TaskInfoExtendedEntry
    {
        [JsonPropertyOrder(-850)]
        [JsonPropertyName("TASK_ID")]
        public int TaskID { get; set; }
    }
}
