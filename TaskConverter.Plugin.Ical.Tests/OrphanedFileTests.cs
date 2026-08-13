using System.IO.Abstractions.TestingHelpers;
using Ical.Net.Serialization;
using TaskConverter.Plugin.Ical.Tests.Builder;
using TaskConverter.Plugin.Ical.Tests.Utils;
using Xunit;

namespace TaskConverter.Plugin.Ical.Tests;

/// <summary>
/// A repeated run has to notice what disappeared from the source, otherwise a task deleted in the
/// source app stays behind as an .ics forever and the destination grows with every run.
/// <para>
/// Only files this converter wrote are candidates, recognised by the PRODID it stamps on them.
/// The destination may be a calendar collection holding other people's data, and a converter has
/// no business touching files it did not create.
/// </para>
/// </summary>
public class OrphanedFileTests
{
    private const string Destination = "/destination";
    private readonly CalendarSerializer _calendarSerializer = new();

    [Fact]
    public void Write_ReportsOrphanedFile_ButKeepsItByDefault()
    {
        var fileSystem = PrepareDestination();
        var orphanPath = WriteOurFile(fileSystem, "gone.ics");

        Write(fileSystem, deleteOrphanedFiles: false, "stillthere");

        Assert.True(fileSystem.File.Exists(orphanPath));
    }

    [Fact]
    public void Write_DeletesOrphanedFile_WhenConfigured()
    {
        var fileSystem = PrepareDestination();
        var orphanPath = WriteOurFile(fileSystem, "gone.ics");

        Write(fileSystem, deleteOrphanedFiles: true, "stillthere");

        Assert.False(fileSystem.File.Exists(orphanPath));
    }

    [Fact]
    public void Write_KeepsTheFilesItJustWrote()
    {
        var fileSystem = PrepareDestination();

        Write(fileSystem, deleteOrphanedFiles: true, "stillthere");

        Assert.True(fileSystem.File.Exists(fileSystem.Path.Combine(Destination, "stillthere.ics")));
    }

    /// <summary>
    /// The decisive one: a calendar file written by anything else must survive, even with deletion
    /// switched on.
    /// </summary>
    [Fact]
    public void Write_NeverTouchesForeignFiles()
    {
        var fileSystem = PrepareDestination();
        var foreignPath = fileSystem.Path.Combine(Destination, "foreign.ics");
        fileSystem.AddFile(foreignPath, new MockFileData("BEGIN:VCALENDAR\r\nPRODID:-//Some Other Tool//EN\r\nEND:VCALENDAR\r\n"));
        var hiddenPath = fileSystem.Path.Combine(Destination, ".Radicale.props");
        fileSystem.AddFile(hiddenPath, new MockFileData("{}"));

        Write(fileSystem, deleteOrphanedFiles: true, "stillthere");

        Assert.True(fileSystem.File.Exists(foreignPath));
        Assert.True(fileSystem.File.Exists(hiddenPath));
    }

    [Fact]
    public void Write_StampsItsOwnProductId()
    {
        var fileSystem = PrepareDestination();

        Write(fileSystem, deleteOrphanedFiles: false, "stillthere");

        var content = fileSystem.GetFile(fileSystem.Path.Combine(Destination, "stillthere.ics")).TextContents;
        Assert.Contains(IcalWriter.ProductId, content);
    }

    private void Write(MockFileSystem fileSystem, bool deleteOrphanedFiles, params string[] uids)
    {
        var settingsProvider = new TestSettingsProvider { DeleteOrphanedFiles = deleteOrphanedFiles };
        var writer = new IcalWriter(fileSystem, _calendarSerializer, settingsProvider);
        var calendars = uids.Select(uid => Create.A.Calendar().AddTodo(uid, $"Task {uid}", DateTime.UtcNow.AddDays(5)).Build()).ToList();

        writer.Write(Destination, calendars);
    }

    /// <summary>Writes a file that carries our PRODID, i.e. one a previous run would have left.</summary>
    private string WriteOurFile(MockFileSystem fileSystem, string fileName)
    {
        var path = fileSystem.Path.Combine(Destination, fileName);
        fileSystem.AddFile(path, new MockFileData($"BEGIN:VCALENDAR\r\nPRODID:{IcalWriter.ProductId}\r\nEND:VCALENDAR\r\n"));
        return path;
    }

    private static MockFileSystem PrepareDestination()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddDirectory(Destination);
        return fileSystem;
    }
}
