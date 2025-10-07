using Ical.Net;
using TaskConverter.Plugin.Base;

namespace TaskConverter.Plugin.Ical;

public class ConversionService(ISettingsProvider settingsProvider) : IConversionService<List<Calendar>>
{
    public ISettingsProvider SettingsProvider { get; } = settingsProvider;

    public Calendar MapToIntermediateFormat(List<Calendar> allCalendars)
    {
        var mergedCalendar = new Calendar();

        foreach (var cal in allCalendars)
        {
            foreach (var component in cal.UniqueComponents)
            {
                mergedCalendar.UniqueComponents.Add(component);
            }
        }

        return mergedCalendar;
    }

    public List<Calendar> MapFromIntermediateFormat(Calendar model)
    {
        var calendarList = new List<Calendar>();
        foreach (var component in model.UniqueComponents)
        {
            var calendar = new Calendar();
            calendar.UniqueComponents.Add(component);
            calendarList.Add(calendar);
        }
        return calendarList;
    }
}
