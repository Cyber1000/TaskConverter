using AutoMapper;
using Converter.Core.GTD.InternalModel;
using Converter.Core.GTD.Model;
using NodaTime;

namespace Converter.Core.Mapper;

public class ReminderResolver : IValueResolver<TaskModel, TaskInfoTaskEntry, long>
{
    int[] fixedDiffsFromDueDate =
    {
        0,
        1,
        5,
        10,
        15,
        30,
        45,
        60,
        90,
        120,
        180,
        240,
        360,
        480,
        600,
        720,
        1080,
        1440,
        2880,
        4320,
        5760,
        7200,
        8640,
        10080,
        20160,
        43200
    };

    public DateTimeZone DateTimeZone { get; }

    public ReminderResolver(DateTimeZone dateTimeZone)
    {
        DateTimeZone = dateTimeZone;
    }

    public long Resolve(TaskModel source, TaskInfoTaskEntry destination, long destMember, ResolutionContext context)
    {
        var reminder = source.Reminder;
        if (reminder == null)
            return -1;

        if (reminder.ReminderInstantType == BaseDateOfReminderInstant.FromUnixEpoch)
        {
            return reminder.MillisecondsFromBaseDate;
        }
        else if (!source.DueDate.HasValue)
        {
            return -1;
        }
        else
        {
            var dueDate = source.DueDate.Value.InZoneLeniently(DateTimeZone).ToInstant();
            foreach (var fixedDiffFromDueDate in fixedDiffsFromDueDate)
            {
                var durationInMiliseconds = Duration.FromMinutes(fixedDiffFromDueDate);
                if (dueDate.Plus(-durationInMiliseconds) == reminder.AbsoluteInstant)
                {
                    return fixedDiffFromDueDate;
                }
            }
            return reminder.AbsoluteInstant.HasValue ? reminder.AbsoluteInstant.Value.ToUnixTimeMilliseconds() : -1;
        }
    }
}
