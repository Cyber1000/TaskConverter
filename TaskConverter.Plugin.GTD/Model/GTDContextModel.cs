using System.Text.Json.Serialization;

namespace TaskConverter.Plugin.GTD.Model;

public class GTDContextModel : GTDBaseWithParentModel
{
    [JsonPropertyOrder(-840)]
    public int Children { get; set; }
}
