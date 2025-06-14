using System.Text.Json.Serialization;
using NodaTime;
using TaskConverter.Plugin.GTD.ConversionHelper;

namespace TaskConverter.Plugin.GTD.Model;

public abstract class GTDBaseModel
{
    [JsonPropertyOrder(-1000)]
    public int Id { get; set; }

    [JsonPropertyOrder(-900)]
    public string? Uuid { get; set; }

    [JsonPropertyOrder(-800)]
    public LocalDateTime Created { get; set; }

    [JsonPropertyOrder(-700)]
    public LocalDateTime Modified { get; set; }

    [JsonPropertyOrder(-600)]
    public string? Title { get; set; }
}
