using System.Text.Json.Serialization;

namespace Converter.Core.GTD.Model;

public abstract class TaskInfoEntryBase
{
    [JsonPropertyOrder(-1000)]
    public int Id { get; set; }

    [JsonPropertyOrder(-900)]
    public string? Uuid { get; set; }

    [JsonPropertyOrder(-800)]
    public DateTime Created { get; set; }

    [JsonPropertyOrder(-700)]
    public DateTime Modified { get; set; }

    [JsonPropertyOrder(-600)]
    public string? Title { get; set; }
}
