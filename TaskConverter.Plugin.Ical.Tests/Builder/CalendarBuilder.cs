using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;

namespace TaskConverter.Plugin.Ical.Tests.Builder;

public static class CalendarBuilderExtensions
{
    public static CalendarBuilder Calendar(this IObjectBuilder _) => new();
}

public class CalendarBuilder
{
    private readonly List<Todo> _todos = [];
    private readonly List<Journal> _journals = [];

    public CalendarBuilder AddTodo(string uid, string summary, DateTime due, string status = "NEEDS-ACTION")
    {
        _todos.Add(
            new Todo
            {
                Uid = uid,
                Summary = summary,
                Due = new CalDateTime(due),
                Status = status,
                DtStamp = new CalDateTime(DateTime.UtcNow),
            }
        );
        return this;
    }

    public CalendarBuilder AddJournal(string uid, string summary, string description)
    {
        _journals.Add(
            new Journal
            {
                Uid = uid,
                Summary = summary,
                Description = description,
                DtStamp = new CalDateTime(DateTime.UtcNow),
            }
        );
        return this;
    }

    public Calendar Build()
    {
        var calendar = new Calendar { Version = "2.0", ProductId = "-//Test//CalendarBuilder//EN" };
        calendar.Todos.AddRange(_todos);
        _journals.ForEach(j => calendar.Journals.Add(j));
        return calendar;
    }
}
