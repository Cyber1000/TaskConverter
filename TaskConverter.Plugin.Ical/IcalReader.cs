using System.IO.Abstractions;
using Ical.Net;
using TaskConverter.Plugin.Base;

namespace TaskConverter.Plugin.Ical;

public class IcalReader : IReader<List<Calendar>?>
{
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
    }

    public (bool isError, string validationError) CheckSource()
    {
        //TODO HH: fix
        throw new NotImplementedException();
    }
}
