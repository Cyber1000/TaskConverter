using System.Text.Json.Serialization;

namespace TaskConverter.Plugin.GTD.Model;

public class GTDDataModel
{
    //TODO HH: better general property?
    [JsonIgnore]
    public List<GTDBaseModel> GetAllEntries
    {
        get
        {
            var items = Folder
                ?.Cast<GTDBaseModel>()
                .Concat(Tag?.Cast<GTDBaseModel>() ?? new List<GTDBaseModel>())
                .Concat(Task?.Cast<GTDBaseModel>() ?? new List<GTDBaseModel>())
                .Concat(Context?.Cast<GTDBaseModel>() ?? new List<GTDBaseModel>())
                .Concat(Notebook?.Cast<GTDBaseModel>() ?? new List<GTDBaseModel>())
                .Concat(TaskNote?.Cast<GTDBaseModel>() ?? new List<GTDBaseModel>());

            return items?.ToList() ?? [];
        }
    }

    [JsonPropertyName("version")]
    public int Version { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<GTDFolderModel>? Folder { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<GTDContextModel>? Context { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<GTDTagModel>? Tag { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<GTDTaskModel>? Task { get; set; }

    [JsonPropertyName("TASKNOTE")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<GTDTaskNoteModel>? TaskNote { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<GTDNotebookModel>? Notebook { get; set; }

    [JsonPropertyName("Preferences")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<GTDPreferencesModel>? Preferences { get; set; }
}
