using AutoMapper;
using Ical.Net.CalendarComponents;
using NodaTime;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Utils;

namespace TaskConverter.Plugin.GTD.Conversion;

public class MapAlarmFromIntermediateFormat(IClock clock, DateTimeZone dateTimeZone) : IValueResolver<Todo, GTDTaskModel, LocalDateTime?>
{
    public DateTimeZone DateTimeZone { get; } = dateTimeZone;

    public LocalDateTime? Resolve(Todo source, GTDTaskModel destination, LocalDateTime? destMember, ResolutionContext context)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);
        ArgumentNullException.ThrowIfNull(context);

        var settingsProvider = context.GetSettingsProvider();
        if (source.Alarms?.Count > 1)
        {
            if (settingsProvider.AllowIncompleteMappingIfMoreThanOneItem())
                Console.WriteLine("More than one Alarm, can only convert the first.");
            else
                throw new Exception("More than one Alarm. This is only allowed if AllowIncompleteMappingIfMoreThanOneItem is true.");
        }
        var alarm = source.Alarms?.FirstOrDefault()?.Trigger;

        var currentDateTime = clock.GetCurrentInstant();
        if (alarm?.DateTime == null)
        {
            return null;
        }

        var absoluteInstant = Instant.FromDateTimeUtc(alarm.DateTime.Value.ToUniversalTime());

        return absoluteInstant <= currentDateTime ? absoluteInstant.InZone(DateTimeZone).LocalDateTime : null;
    }
}
