using AutoMapper;
using Ical.Net.CalendarComponents;
using NodaTime;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Mapper;

public class AlarmResolver(IClock clock, DateTimeZone dateTimeZone) : IValueResolver<Todo, GTDTaskModel, LocalDateTime?>
{
    public DateTimeZone DateTimeZone { get; } = dateTimeZone;

    public LocalDateTime? Resolve(Todo source, GTDTaskModel destination, LocalDateTime? destMember, ResolutionContext context)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);
        ArgumentNullException.ThrowIfNull(context);

        //TODO HH: FirstOrDefault not exact
        var alarm = source.Alarms?.FirstOrDefault()?.Trigger;

        var currentDateTime = clock.GetCurrentInstant();
        if (alarm?.DateTime == null)
        {
            return null;
        }

        var absoluteInstant = Instant.FromDateTimeUtc(alarm.DateTime.Value);

        return absoluteInstant <= currentDateTime ? absoluteInstant.InZone(DateTimeZone).LocalDateTime : null;
    }
}
