using System.Text.Json.Serialization;

namespace TaskConverter.Plugin.GTD.Model;

public abstract class GTDExtendedModel : GTDBaseModel
{
    [JsonPropertyOrder(-500)]
    public int Color { get; set; }

    [JsonPropertyOrder(-400)]
    public bool Visible { get; set; }
}
