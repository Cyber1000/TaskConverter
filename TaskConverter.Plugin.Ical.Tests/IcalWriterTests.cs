using System.IO.Abstractions.TestingHelpers;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.Serialization;
using TaskConverter.Plugin.Ical.Tests.Builder;
using TaskConverter.Plugin.Ical.Tests.Utils;
using Xunit;

namespace TaskConverter.Plugin.Ical.Tests;

public class IcalWriterTests
{
    private readonly CalendarSerializer _calendarSerializer = new();
    
    [Fact]
    public void Write_ShouldNotThrow_WhenModelIsNull()
    {
        var mockFileSystem = PrepareDestination();
        var writer = new IcalWriter(mockFileSystem, _calendarSerializer);

        var ex = Record.Exception(() => writer.Write("/destination", null));

        Assert.Null(ex);
        Assert.Empty(mockFileSystem.AllFiles);
    }

    [Fact]
    public void Write_ShouldWriteFile_WithSerializedCalendar()
    {
        var mockFileSystem = PrepareDestination();
        var calendar = Create.A.Calendar().AddTodo("event1", "Test Task 1", DateTime.UtcNow.AddDays(5)).Build();

        var writer = new IcalWriter(mockFileSystem, _calendarSerializer);
        string destination = "/destination";

        writer.Write(destination, [calendar]);

        var expectedPath = mockFileSystem.Path.Combine(destination, "event1.ics");
        Assert.Contains(expectedPath, mockFileSystem.AllFiles);

        var savedContent = mockFileSystem.GetFile(expectedPath).TextContents;
        Assert.Contains("BEGIN:VCALENDAR", savedContent);
        Assert.Contains("UID:event1", savedContent);
    }

    [Fact]
    public void Write_ShouldThrowException_WhenSerializedCalendarIsNull()
    {
        var mockFileSystem = PrepareDestination();
        var calendar = Create.A.Calendar().AddTodo("event1", "Test Task 1", DateTime.UtcNow.AddDays(5)).Build();

        var writer = new IcalWriter(mockFileSystem, new NullCalendarSerializer());
        string destination = "/destination";

        var ex = Assert.Throws<Exception>(() => writer.Write(destination, [calendar]));
        Assert.Equal("Should not be null after serialization.", ex.Message);
    }

    [Fact]
    public void Write_ShouldThrowException_WhenUidIsNull()
    {
        var mockFileSystem = PrepareDestination();
        var writer = new IcalWriter(mockFileSystem, _calendarSerializer);

        var calendar = new Calendar();
        calendar.UniqueComponents.Add(new Todo { Uid = null });

        var ex = Assert.Throws<Exception>(() => writer.Write("dest", [calendar]));
        Assert.Equal("No Uid found for calendar.", ex.Message);
    }

    private static MockFileSystem PrepareDestination()
    {
        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddDirectory("/destination");
        return mockFileSystem;
    }
}
