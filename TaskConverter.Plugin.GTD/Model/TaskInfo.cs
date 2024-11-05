using System.Text.Json.Serialization;

namespace TaskConverter.Plugin.GTD.Model;

public class TaskInfo
{
    [JsonIgnore]
    public List<TaskInfoEntryBase> GetAllEntries
    {
        get
        {
            var items = Folder
                ?.Cast<TaskInfoEntryBase>()
                .Concat(Tag?.Cast<TaskInfoEntryBase>() ?? new List<TaskInfoEntryBase>())
                .Concat(Task?.Cast<TaskInfoEntryBase>() ?? new List<TaskInfoEntryBase>())
                .Concat(Context?.Cast<TaskInfoEntryBase>() ?? new List<TaskInfoEntryBase>())
                .Concat(Notebook?.Cast<TaskInfoEntryBase>() ?? new List<TaskInfoEntryBase>())
                .Concat(TaskNote?.Cast<TaskInfoEntryBase>() ?? new List<TaskInfoEntryBase>());

            return items?.ToList() ?? [];
        }
    }

    [JsonPropertyName("version")]
    public int Version { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<TaskInfoFolderEntry>? Folder { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<TaskInfoContextEntry>? Context { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<TaskInfoTagEntry>? Tag { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<TaskInfoTaskEntry>? Task { get; set; }

    [JsonPropertyName("TASKNOTE")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<TaskInfoTaskNote>? TaskNote { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<TaskInfoNotebook>? Notebook { get; set; }

    [JsonPropertyName("Preferences")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Preferences>? Preferences { get; set; }
}
