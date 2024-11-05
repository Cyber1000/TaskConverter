using AutoMapper;
using TaskConverter.Model.Model;
using TaskConverter.Plugin.GTD.Model;
using NodaTime;

namespace TaskConverter.Plugin.GTD.Mapper;

public class AlarmResolver(IClock clock, DateTimeZone dateTimeZone) : IValueResolver<TaskModel, TaskInfoTaskEntry, LocalDateTime?>
{
    public DateTimeZone DateTimeZone { get; } = dateTimeZone;

    public LocalDateTime? Resolve(TaskModel source, TaskInfoTaskEntry destination, LocalDateTime? destMember, ResolutionContext context)
    {
        var currentDateTime = clock.GetCurrentInstant();
        var reminder = source.Reminder;
        if (reminder == null)
            return null;
        if (reminder.AbsoluteInstant.HasValue && reminder.AbsoluteInstant <= currentDateTime)
            return reminder.AbsoluteInstant.Value.InZone(DateTimeZone).LocalDateTime;
        else
            return null;
    }
}
