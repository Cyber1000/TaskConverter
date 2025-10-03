using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;

namespace TaskConverter.Plugin.GTD.Tests.Utils;

public static class CalendarBuilderExtensions
{
    public static TodoBuilder Todo(this IObjectBuilder _) => new();
}

public class TodoBuilder
{
    public enum StatusEnum
    {
        NeedsAction,
        InProcess,
        Cancelled,
        Completed
    }

    private string _summary = string.Empty;
    private string? _description;
    private CalDateTime _created = new(DateTime.UtcNow);
    private CalDateTime _lastModified = new(DateTime.UtcNow);
    private readonly IList<string> _categories = [];
    private readonly List<RecurrencePattern> _recurrencePatterns = [];
    private readonly List<Alarm> _alarms = [];
    private string _status = "IN-PROCESS";

    public TodoBuilder()
    {
        WithSummary("Test");
        WithDescription("Test");
        WithCreated(new CalDateTime(DateTime.UtcNow));
        WithLastModified(new CalDateTime(DateTime.UtcNow));
    }

    public TodoBuilder WithSummary(string summary)
    {
        _summary = summary;
        return this;
    }

    public TodoBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public TodoBuilder WithCreated(CalDateTime created)
    {
        _created = created ?? throw new ArgumentNullException(nameof(created));
        return this;
    }

    public TodoBuilder WithLastModified(CalDateTime lastModified)
    {
        _lastModified = lastModified ?? throw new ArgumentNullException(nameof(lastModified));
        return this;
    }

    public TodoBuilder WithStatus(StatusEnum status)
    {
        switch (status)
        {
            case StatusEnum.NeedsAction:
                _status = "NEEDS-ACTION";
                break;
            case StatusEnum.InProcess:
                _status = "IN-PROCESS";
                break;
            case StatusEnum.Cancelled:
                _status = "CANCELLED";
                break;
            case StatusEnum.Completed:
                _status = "COMPLETED";
                break;
        }
        return this;
    }

    public TodoBuilder AddRecurrenceRule(RecurrencePattern recurrencePattern)
    {
        _recurrencePatterns.Add(recurrencePattern);
        return this;
    }

    public TodoBuilder AddAlarm(Alarm alarm)
    {
        _alarms.Add(alarm);
        return this;
    }

    public TodoBuilder AddCategory(string category)
    {
        _categories.Add(category);
        return this;
    }

    public Todo Build()
    {
        var todo = new Todo
        {
            Summary = _summary,
            Description = _description,
            Created = _created,
            LastModified = _lastModified,
            RecurrenceRules = _recurrencePatterns,
            Categories = _categories,
            Status = _status,
        };
        _alarms.ForEach(a => todo.Alarms.Add(a));

        return todo;
    }
}
