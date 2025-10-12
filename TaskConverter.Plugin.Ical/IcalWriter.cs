using System.IO.Abstractions;
using System.Text;
using Ical.Net;
using Ical.Net.Serialization;
using TaskConverter.Plugin.Base;

namespace TaskConverter.Plugin.Ical;

public class IcalWriter(IFileSystem FileSystem) : IWriter<List<Calendar>?>
{
    public void Write(string destination, List<Calendar>? model)
    {
        if (model == null)
            return;

        var serializer = new CalendarSerializer();
        foreach (var calendar in model)
        {
            string serializedCalendar = serializer.SerializeToString(calendar) ?? throw new Exception("Should not be null after serialization.");

            var fileUid = calendar.UniqueComponents.First().Uid ?? throw new Exception("No Uid found for calendar.");
            var filePath = FileSystem.Path.Combine(destination, $"{fileUid}.ics");
            FileSystem.File.WriteAllText(filePath, serializedCalendar, Encoding.UTF8);
        }
    }
}
