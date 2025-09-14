using Ical.Net;
using Ical.Net.CalendarComponents;
using TaskConverter.Plugin.GTD.Tests.Utils;

namespace TaskConverter.Plugin.GTD.Tests.MappingTests;

public static class CalendarBuilderExtensions
{
    public static CalendarBuilder Calendar(this IObjectBuilder _) => new();
}

public class CalendarBuilder
{
    private Func<IEnumerable<Todo>> _todoListFunc = () => Enumerable.Empty<Todo>();

    public CalendarBuilder WithTask(Todo todo)
    {
        ArgumentNullException.ThrowIfNull(todo);

        var existing = _todoListFunc;
        _todoListFunc = () => existing().Append(todo);
        return this;
    }

    public Calendar Build()
    {
        var calendar = new Calendar();
        foreach (var todo in _todoListFunc())
        {
            calendar.Todos.Add(todo);
        }
        return calendar;
    }
}
