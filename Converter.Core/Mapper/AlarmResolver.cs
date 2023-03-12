using AutoMapper;
using Converter.Core.GTD.InternalModel;
using Converter.Core.GTD.Model;
using NodaTime;

namespace Converter.Core.Mapper;

public class AlarmResolver : IValueResolver<TaskModel, TaskInfoTaskEntry, LocalDateTime?>
{
    public DateTimeZone DateTimeZone { get; }
    private readonly IClock Clock;

    public AlarmResolver(IClock clock, DateTimeZone dateTimeZone)
    {
        Clock = clock;
        DateTimeZone = dateTimeZone;
    }

    public LocalDateTime? Resolve(TaskModel source, TaskInfoTaskEntry destination, LocalDateTime? destMember, ResolutionContext context)
    {
        var currentDateTime = Clock.GetCurrentInstant();
        var reminder = source.Reminder;
        if (reminder == null)
            return null;
        if (reminder.AbsoluteInstant.HasValue && reminder.AbsoluteInstant <= currentDateTime)
            return reminder.AbsoluteInstant.Value.InZone(DateTimeZone).LocalDateTime;
        else
            return null;
    }
}
