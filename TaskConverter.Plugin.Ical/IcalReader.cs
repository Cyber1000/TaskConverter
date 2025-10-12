using System.IO.Abstractions;
using Ical.Net;
using TaskConverter.Commons;
using TaskConverter.Plugin.Base;

namespace TaskConverter.Plugin.Ical;

public class IcalReader(IFileSystem FileSystem) : IReader<List<Calendar>?>
{
    public List<Calendar> Read(string source)
    {
        var result = new List<Calendar>();

        foreach (var fileInfo in GetDirectoryInfo(FileSystem, source).GetFiles("*.ics"))
        {
            var icsText = FileSystem.File.ReadAllText(fileInfo.FullName);
            var calendar = Calendar.Load(icsText);
            if (calendar != null)
                result.Add(calendar);
        }
        return result;
    }

    public SourceResult CheckSource(string source)
    {
        try
        {
            return (GetDirectoryInfo(FileSystem, source).GetFiles("*.ics").Length != 0) ? new SourceResult(true, null) : new SourceResult(false, new Exception("No ics-file found."));
        }
        catch (Exception ex)
        {
            return new SourceResult(false, ex);
        }
    }

    private static IDirectoryInfo GetDirectoryInfo(IFileSystem FileSystem, string source) => FileSystem.DirectoryInfo.New(source);
}
