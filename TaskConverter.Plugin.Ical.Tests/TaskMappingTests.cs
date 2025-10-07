using Ical.Net;
using Ical.Net.CalendarComponents;
using TaskConverter.Plugin.Base;
using TaskConverter.Plugin.Ical.Tests.Builder;
using Xunit;

namespace TaskConverter.Plugin.Ical.Tests;

public class TaskMappingTests(IConversionService<List<Calendar>> testConverter)
{
    protected readonly IConversionService<List<Calendar>> TestConverter = testConverter;

    [Fact]
    public void Map_ToIntermediateFormat_ShouldBeValid()
    {
        var calendar1 = Create.A.Calendar().AddTodo("task-123@example.com", "Test Task 1", DateTime.UtcNow.AddDays(5)).Build();

        var calendar2 = Create.A.Calendar().AddJournal("journal-456@example.com", "Test Journal", "Details for journal").Build();

        var calendarList = new List<Calendar> { calendar1, calendar2 };
        var result = TestConverter.MapToIntermediateFormat(calendarList);

        Assert.Equal(2, result.UniqueComponents.Count);
        Assert.Single(result.Todos);
        Assert.Single(result.Journals);
        Assert.Equal(calendar1.Todos.First(), result.Todos.First());
        Assert.Equal(calendar2.Journals.First(), result.Journals.First());
    }

    [Fact]
    public void Map_FromIntermediateFormat_ShouldBeValid()
    {
        var calendar = Create.A.Calendar()
        .AddTodo("task-123@example.com", "Test Task 1", DateTime.UtcNow.AddDays(5))
        .AddJournal("journal-456@example.com", "Test Journal", "Details for journal")
        .Build();

        var result = TestConverter.MapFromIntermediateFormat(calendar);

        Assert.Equal(2, result.Count);
        Assert.Equal(calendar.Todos.First(), result.SelectMany(r => r.Todos).First());
        Assert.Equal(calendar.Journals.First(), result.SelectMany(r => r.Journals).First());
    }
}
