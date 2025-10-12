using System.IO.Abstractions.TestingHelpers;
using Ical.Net;
using Ical.Net.CalendarComponents;
using TaskConverter.Plugin.Ical.Tests.Builder;
using Xunit;

namespace TaskConverter.Plugin.Ical.Tests;

public class IcalWriterTests
{
    [Fact]
    public void Write_ShouldNotThrow_WhenModelIsNull()
    {
        var mockFileSystem = PrepareDestination();
        var writer = new IcalWriter(mockFileSystem);

        var ex = Record.Exception(() => writer.Write("/destination", null));

        Assert.Null(ex);
        Assert.Empty(mockFileSystem.AllFiles);
    }

    [Fact]
    public void Write_ShouldWriteFile_WithSerializedCalendar()
    {
        var mockFileSystem = PrepareDestination();
        var calendar = Create.A.Calendar().AddTodo("event1", "Test Task 1", DateTime.UtcNow.AddDays(5)).Build();

        var writer = new IcalWriter(mockFileSystem);
        string destination = "/destination";

        writer.Write(destination, new List<Calendar> { calendar });

        var expectedPath = mockFileSystem.Path.Combine(destination, "event1.ics");
        Assert.Contains(expectedPath, mockFileSystem.AllFiles);

        var savedContent = mockFileSystem.GetFile(expectedPath).TextContents;
        Assert.Contains("BEGIN:VCALENDAR", savedContent);
        Assert.Contains("UID:event1", savedContent);
    }

    [Fact]
    public void Write_ShouldThrowException_WhenUidIsNull()
    {
        var mockFileSystem = PrepareDestination();
        var writer = new IcalWriter(mockFileSystem);

        var calendar = new Calendar();
        calendar.UniqueComponents.Add(new Todo { Uid = null });

        var ex = Assert.Throws<Exception>(() => writer.Write("dest", new List<Calendar> { calendar }));
        Assert.Equal("No Uid found for calendar.", ex.Message);
    }

    private static MockFileSystem PrepareDestination()
    {
        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddDirectory("/destination");
        return mockFileSystem;
    }
}
