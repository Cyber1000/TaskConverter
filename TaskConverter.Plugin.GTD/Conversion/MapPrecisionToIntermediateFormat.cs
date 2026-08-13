using AutoMapper;
using Ical.Net.CalendarComponents;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Conversion;

public class MapPrecisionToIntermediateFormat : IMappingAction<GTDBaseModel, RecurringComponent>
{
    public void Process(GTDBaseModel source, RecurringComponent destination, ResolutionContext context)
    {
        AddMillisecondsIfAny(destination, IntermediateFormatPropertyNames.CreatedMilliseconds, source.Created.Millisecond);
        AddMillisecondsIfAny(destination, IntermediateFormatPropertyNames.ModifiedMilliseconds, source.Modified.Millisecond);
    }

    internal static void AddMillisecondsIfAny(RecurringComponent destination, string propertyName, int milliseconds)
    {
        if (milliseconds != 0)
            destination.AddProperty(propertyName, milliseconds.ToString());
    }
}
