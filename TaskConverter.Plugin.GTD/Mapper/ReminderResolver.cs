using AutoMapper;
using Ical.Net.CalendarComponents;
using NodaTime;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Mapper;

public class ReminderResolver(DateTimeZone dateTimeZone) : IValueResolver<Todo, GTDTaskModel, long>
{
    readonly int[] fixedDiffsFromDueDate = [0, 1, 5, 10, 15, 30, 45, 60, 90, 120, 180, 240, 360, 480, 600, 720, 1080, 1440, 2880, 4320, 5760, 7200, 8640, 10080, 20160, 43200];

    public DateTimeZone DateTimeZone { get; } = dateTimeZone;

    public long Resolve(Todo source, GTDTaskModel destination, long destMember, ResolutionContext context)
    {
        //TODO HH: FirstOrDefault not exact
        var alarm = source.Alarms?.FirstOrDefault()?.Trigger;
        if (alarm == null)
            return -1;

        if (alarm.DateTime != null)
        {
            return new DateTimeOffset(alarm.DateTime.Value).ToUnixTimeMilliseconds();
        }
        else if (source.Due == null)
        {
            return -1;
        }
        else
        {
            var dueDate = new DateTimeOffset(source.Due.Value).ToUnixTimeMilliseconds();
            if (alarm.Duration.HasValue)
            {
                // attention: alarm.Duration is negative, so we have to invert this here
                var duration = -alarm.Duration.Value;
                foreach (var fixedDiffFromDueDateInMinutes in fixedDiffsFromDueDate)
                {
                    if (fixedDiffFromDueDateInMinutes == duration.TotalMinutes)
                    {
                        return fixedDiffFromDueDateInMinutes;
                    }
                }
                return dueDate - (long)duration.TotalMilliseconds;
            }
            return -1;
        }
    }
}
