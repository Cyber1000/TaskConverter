using AutoMapper;
using Converter.Core.GTD.InternalModel;
using Converter.Core.GTD.Model;
using NodaTime;

namespace Converter.Core.Mapper;

public class AlarmResolver : IValueResolver<TaskModel, TaskInfoTaskEntry, LocalDateTime?>
{
    public DateTimeZone DateTimeZone { get; }

    public AlarmResolver(DateTimeZone dateTimeZone)
    {
        DateTimeZone = dateTimeZone;
    }

    public LocalDateTime? Resolve(TaskModel source, TaskInfoTaskEntry destination, LocalDateTime? destMember, ResolutionContext context)
    {
        //TODO HH: improvement - Inject Fake-SystemClock for better Testing, adjust Map_TaskWithAlarm_ShouldBeValid
        var currentDateTime = SystemClock.Instance.GetCurrentInstant();
        var reminder = source.Reminder;
        if (reminder == null)
            return null;
        if (reminder.AbsoluteInstant.HasValue && reminder.AbsoluteInstant <= currentDateTime)
            return reminder.AbsoluteInstant.Value.InZone(DateTimeZone).LocalDateTime;
        else
            return null;
    }
}
