using System.Text.Json.Serialization;
using Converter.Commons.ConversionHelper;

namespace Converter.Plugin.GTD.Model
{
    public class TaskInfoNotebook : TaskInfoExtendedEntry
    {
        [JsonPropertyOrder(-650)]
        public int Private { get; set; }

        [JsonPropertyOrder(-590)]
        [JsonConverter(typeof(TaskInfoStringArrayConverter))]
        public string[]? Note { get; set; }

        [JsonPropertyOrder(-580)]
        [JsonPropertyName("FOLDER_ID")]
        public int FolderId { get; set; }
    }
}
