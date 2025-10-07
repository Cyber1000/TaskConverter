using System.IO.Abstractions;
using Ical.Net;
using TaskConverter.Plugin.Base;

namespace TaskConverter.Plugin.Ical;

public class IcalReader : IReader<List<Calendar>?>
{
    private IDirectoryInfo DirectoryInfo { get; }

    public List<Calendar> Result { get; } = [];

    public IcalReader(IFileSystem fileSystem, IDirectoryInfo directoryInfo)
    {
        foreach (var fileInfo in directoryInfo.GetFiles("*.ics"))
        {
            var icsText = fileSystem.File.ReadAllText(fileInfo.FullName);
            var calendar = Calendar.Load(icsText);
            if (calendar != null)
                Result.Add(calendar);
        }

        DirectoryInfo = directoryInfo;
    }

    public (bool isError, string validationError) CheckSource()
    {
        try
        {
            return (DirectoryInfo.GetFiles("*.ics").Length != 0) ? (false, string.Empty) : (true, "No ics-file found.");
        }
        catch (Exception ex)
        {
            return (true, ex.Message);
        }
    }
}
