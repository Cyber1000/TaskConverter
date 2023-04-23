using AutoMapper;
using Converter.Model.Model;
using Converter.Plugin.GTD.Model;
using NodaTime;

namespace Converter.Plugin.GTD.Mapper;

public class ReminderInstantResolver : IValueResolver<TaskInfoTaskEntry, TaskModel, ReminderInstant?>
{
    private DateTimeZone DateTimeZone { get; }

    public ReminderInstantResolver(DateTimeZone dateTimeZone)
    {
        DateTimeZone = dateTimeZone;
    }

    public ReminderInstant? Resolve(TaskInfoTaskEntry source, TaskModel destination, ReminderInstant? destMember, ResolutionContext context)
    {
        if (source.Reminder > 43200)
            return new ReminderInstant(destination, DateTimeZone, BaseDateOfReminderInstant.FromUnixEpoch, source.Reminder);
        else if (source.Reminder >= 0)
            return new ReminderInstant(destination, DateTimeZone, BaseDateOfReminderInstant.FromDueDate, -(source.Reminder * 60 * 1000));

        return null;
    }
}
