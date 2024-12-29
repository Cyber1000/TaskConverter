using AutoMapper;
using TaskConverter.Model.Model;
using TaskConverter.Plugin.GTD.Model;
using NodaTime;

namespace TaskConverter.Plugin.GTD.Mapper;

public class AlarmResolver(IClock clock, DateTimeZone dateTimeZone) : IValueResolver<TaskModel, GTDTaskModel, LocalDateTime?>
{
    public DateTimeZone DateTimeZone { get; } = dateTimeZone;

    public LocalDateTime? Resolve(TaskModel source, GTDTaskModel destination, LocalDateTime? destMember, ResolutionContext context)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);
        ArgumentNullException.ThrowIfNull(context);

        var currentDateTime = clock.GetCurrentInstant();
        if (source.Reminder?.AbsoluteInstant is not { } absoluteInstant)
        {
            return null;
        }

        return absoluteInstant <= currentDateTime 
            ? absoluteInstant.InZone(DateTimeZone).LocalDateTime 
            : null;
    }
}
