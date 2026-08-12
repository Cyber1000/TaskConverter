using AutoMapper;
using Ical.Net.CalendarComponents;
using NodaTime;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Utils;

namespace TaskConverter.Plugin.GTD.Conversion;

public class MapAlarmFromIntermediateFormat(DateTimeZone dateTimeZone) : IValueResolver<Todo, GTDTaskModel, LocalDateTime?>
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
        // The carried value is authoritative: Alarm and Reminder share the single VALARM, so an
        // absolute trigger may just as well belong to the reminder.
        if (source.Properties.Contains(IntermediateFormatPropertyNames.Alarm))
            return source.Properties.GetCalDateTime(IntermediateFormatPropertyNames.Alarm)?.GetLocalDateTime(DateTimeZone);

        var alarm = source.Alarms?.FirstOrDefault()?.Trigger;
        if (alarm?.DateTime == null)
        {
            return null;
        }

        // An alarm is transported whether or not it has already passed. Comparing against the clock
        // made the result depend on when the conversion ran: the same file converted differently
        // tomorrow, and 18 tasks in the backup lost their alarm because it lies in the future.
        return Instant.FromDateTimeUtc(alarm.DateTime.Value.ToUniversalTime()).InZone(DateTimeZone).LocalDateTime;
    }
}
