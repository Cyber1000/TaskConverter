using System.IO.Abstractions.TestingHelpers;
using TaskConverter.Plugin.Ical.Tests.Builder;
using TaskConverter.Plugin.Ical.Tests.Utils;
using Xunit;

namespace TaskConverter.Plugin.Ical.Tests;

public class IcalReaderTests
{
    [Fact]
    public void Read_Ical_ShouldBeValid()
    {
        var calendar1 = Create.A.Calendar()
            .AddTodo("task-123@example.com", "Test Task 1", DateTime.UtcNow.AddDays(5))
            .Build()
            .Serialize();

        var calendar2 = Create.A.Calendar()
            .AddJournal("journal-456@example.com", "Test Journal", "Details for journal")
            .Build()
            .Serialize();

        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { @"/TestFolder/file1.ics", new MockFileData(calendar1) },
            { @"/TestFolder/file2.ics", new MockFileData(calendar2) }
        });

        var directoryInfo = mockFileSystem.DirectoryInfo.New(@"/TestFolder");
        var reader = new IcalReader(mockFileSystem, directoryInfo);

        Assert.Equal(2, reader.Result.Count);
        Assert.Contains(reader.Result, c => c.Todos.Count == 1);
        Assert.Contains(reader.Result, c => c.Journals.Count == 1);
    }
}
