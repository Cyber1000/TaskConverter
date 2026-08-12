using AutoMapper;
using Ical.Net.CalendarComponents;
using NodaTime;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Conversion;

public class MapPrecisionFromIntermediateFormat : IMappingAction<RecurringComponent, GTDBaseModel>
{
    public void Process(RecurringComponent source, GTDBaseModel destination, ResolutionContext context)
    {
        destination.Created = destination.Created.PlusMilliseconds(GetMilliseconds(source, IntermediateFormatPropertyNames.CreatedMilliseconds));
        destination.Modified = destination.Modified.PlusMilliseconds(GetMilliseconds(source, IntermediateFormatPropertyNames.ModifiedMilliseconds));
    }

    /// <summary>
    /// Returns 0 for a calendar written by a foreign client, which has no such property.
    /// </summary>
    internal static int GetMilliseconds(RecurringComponent source, string propertyName)
    {
        return int.TryParse(source.Properties.Get<string>(propertyName), out var milliseconds) ? milliseconds : 0;
    }
}
