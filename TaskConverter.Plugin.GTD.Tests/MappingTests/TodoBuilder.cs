using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;

namespace TaskConverter.Plugin.GTD.Tests.Utils;

public static class CalendarBuilderExtensions
{
    public static TodoBuilder Todo(this IObjectBuilder _) => new();
}

public class TodoBuilder
{
    private string _summary = string.Empty;
    private string? _description;
    private IDateTime _created = new CalDateTime(DateTime.Now);
    private IDateTime _lastModified = new CalDateTime(DateTime.Now);
    private readonly List<RecurrencePattern> _recurrencePatterns = [];
    private readonly List<Alarm> _alarms = [];

    public TodoBuilder()
    {
        WithSummary("Test");
        WithDescription("Test");
        WithCreated(new CalDateTime(DateTime.Now));
        WithLastModified(new CalDateTime(DateTime.Now));
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

    public TodoBuilder WithCreated(IDateTime created)
    {
        _created = created ?? throw new ArgumentNullException(nameof(created));
        return this;
    }

    public TodoBuilder WithLastModified(IDateTime lastModified)
    {
        _lastModified = lastModified ?? throw new ArgumentNullException(nameof(lastModified));
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

    public Todo Build()
    {
        var todo = new Todo
        {
            Summary = _summary,
            Description = _description,
            Created = _created,
            LastModified = _lastModified,
            RecurrenceRules = _recurrencePatterns,
        };
        _alarms.ForEach(a => todo.Alarms.Add(a));

        return todo;
    }
}
