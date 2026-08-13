using System.IO.Abstractions;
using System.Text;
using Ical.Net;
using Ical.Net.Serialization;
using TaskConverter.Plugin.Base;

namespace TaskConverter.Plugin.Ical;

public class IcalWriter(IFileSystem FileSystem, IStringSerializer stringSerializer, ISettingsProvider settingsProvider) : IWriter<List<Calendar>?>
{
    /// <summary>
    /// Stamped on every file written here, so a later run can tell its own files apart from
    /// everything else in the destination.
    /// </summary>
    public const string ProductId = "-//TaskConverter//Ical Plugin//EN";

    public void Write(string destination, List<Calendar>? model)
    {
        if (model == null)
            return;

        var writtenFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var calendar in model)
        {
            calendar.ProductId = ProductId;
            string serializedCalendar = stringSerializer.SerializeToString(calendar) ?? throw new Exception("Should not be null after serialization.");

            var fileUid = calendar.UniqueComponents.First().Uid ?? throw new Exception("No Uid found for calendar.");
            var fileName = $"{fileUid}.ics";
            FileSystem.File.WriteAllText(FileSystem.Path.Combine(destination, fileName), serializedCalendar, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            writtenFileNames.Add(fileName);
        }

        HandleOrphanedFiles(destination, writtenFileNames);
    }

    /// <summary>
    /// Anything this converter wrote earlier and did not write now no longer exists in the source.
    /// Reporting is the default: the destination may be a shared calendar collection, and deleting
    /// there is the user's decision, not the converter's.
    /// </summary>
    private void HandleOrphanedFiles(string destination, HashSet<string> writtenFileNames)
    {
        var orphanedFiles = FindOrphanedFiles(destination, writtenFileNames);
        if (orphanedFiles.Count == 0)
            return;

        if (!settingsProvider.DeleteOrphanedFiles())
        {
            Console.WriteLine(
                $"{orphanedFiles.Count} file(s) in the destination were written by an earlier run and are no longer in the source: "
                    + $"{string.Join(", ", orphanedFiles.Select(file => file.Name).Take(5))}{(orphanedFiles.Count > 5 ? ", …" : "")}. "
                    + "Set Ical.DeleteOrphanedFiles to remove them."
            );
            return;
        }

        foreach (var orphanedFile in orphanedFiles)
            orphanedFile.Delete();

        Console.WriteLine($"Deleted {orphanedFiles.Count} file(s) that are no longer in the source.");
    }

    private List<IFileInfo> FindOrphanedFiles(string destination, HashSet<string> writtenFileNames)
    {
        var destinationDirectory = FileSystem.DirectoryInfo.New(destination);
        if (!destinationDirectory.Exists)
            return [];

        return
        [
            .. destinationDirectory
                .GetFiles("*.ics")
                .Where(file => !file.Name.StartsWith('.') && !writtenFileNames.Contains(file.Name) && WasWrittenHere(file)),
        ];
    }

    private bool WasWrittenHere(IFileInfo file)
    {
        try
        {
            return FileSystem.File.ReadAllText(file.FullName).Contains(ProductId, StringComparison.Ordinal);
        }
        catch (IOException)
        {
            return false;
        }
    }
}
