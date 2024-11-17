using NodaTime;

namespace TaskConverter.Model.Model;

public abstract class BaseModel
{
    //TODO HH: foreignid?
    public string Id { get; set; } = string.Empty;

    public Instant Created { get; set; }

    public Instant Modified { get; set; }

    public string? Title { get; set; }
}
