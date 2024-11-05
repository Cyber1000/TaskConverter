using AutoMapper;
using TaskConverter.Model.Model;
using TaskConverter.Plugin.GTD.Model;
using NodaTime;

namespace TaskConverter.Plugin.GTD.Mapper;

public class ReminderInstantResolver(DateTimeZone dateTimeZone) : IValueResolver<TaskInfoTaskEntry, TaskModel, ReminderInstant?>
{
    private DateTimeZone DateTimeZone { get; } = dateTimeZone;

    public ReminderInstant? Resolve(TaskInfoTaskEntry source, TaskModel destination, ReminderInstant? destMember, ResolutionContext context)
    {
        if (source.Reminder > 43200)
            return new ReminderInstant(destination, DateTimeZone, BaseDateOfReminderInstant.FromUnixEpoch, source.Reminder);
        else if (source.Reminder >= 0)
            return new ReminderInstant(destination, DateTimeZone, BaseDateOfReminderInstant.FromDueDate, -(source.Reminder * 60 * 1000));

        return null;
    }
}
