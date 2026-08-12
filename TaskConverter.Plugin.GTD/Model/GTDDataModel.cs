using System.Text.Json.Serialization;

namespace TaskConverter.Plugin.GTD.Model;

public class GTDDataModel
{
    [JsonIgnore]
    public List<GTDBaseModel> GetAllEntries
    {
        get
        {
            var entries = new List<GTDBaseModel>();
            AddRange(Folder);
            AddRange(Context);
            AddRange(Tag);
            AddRange(Task);
            AddRange(Notebook);
            AddRange(TaskNote);
            return entries;

            void AddRange<T>(List<T>? items)
                where T : GTDBaseModel
            {
                if (items != null)
                    entries.AddRange(items);
            }
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
