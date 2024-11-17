using System.Text.Json.Serialization;

namespace TaskConverter.Plugin.GTD.Model;

public class GTDFolderModel : GTDBaseWithParentModel
{
    [JsonPropertyOrder(-840)]
    public int Children { get; set; }
    public int Ordinal { get; set; }
}
