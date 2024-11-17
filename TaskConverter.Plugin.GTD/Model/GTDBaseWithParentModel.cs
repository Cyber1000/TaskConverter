using System.Text.Json.Serialization;

namespace TaskConverter.Plugin.GTD.Model;

public abstract class GTDBaseWithParentModel : GTDExtendedModel
{
    [JsonPropertyOrder(-850)]
    public int Parent { get; set; }
}
