using AutoMapper;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using NodaTime;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Utils;
using Period = TaskConverter.Plugin.GTD.Model.Period;

namespace TaskConverter.Plugin.GTD.Conversion;

public class AfterMapTodoToIntermediateFormat : IMappingAction<GTDTaskModel, Todo>
{
    public void Process(GTDTaskModel source, Todo destination, ResolutionContext context)
    {
        MapStartDate(source, destination, context);
        MapStatus(source, destination, context);
        MapAlarm(source, destination);
        MapRecurrenceRule(source, destination, context);
        AddProperties(source, destination);
    }

    private static void MapStartDate(GTDTaskModel source, Todo destination, ResolutionContext context)
    {
        destination.AddProperty(new CalendarProperty(IntermediateFormatPropertyNames.Start, context.Mapper.Map<CalDateTime>(source.StartDate)));
    }

    private static void MapAlarm(GTDTaskModel source, Todo destination)
    {
        var alarm = CreateAlarmFromReminder(source.Reminder);
        if (alarm != null)
            destination.Alarms.Add(alarm);
    }

    private static void MapRecurrenceRule(GTDTaskModel source, Todo destination, ResolutionContext context)
    {
        if (!source.RepeatNew.HasValue)
            return;

        // RFC 5545 (and therefore ical.net) use Start (=DTSTART) as base for recurrence, so we need to set this here this way-
        // Completed may be null and so a fallback of DueDate may be ok here
        // DueDate should not be null if RepeatNew is set, but maybe, therefore find another starting point
        var startOfRecurrence = GetRecurrenceStart(source.RepeatFrom, source.DueDate, source.Completed, source.StartDate, source.Created);
        destination.Start = context.Mapper.Map<CalDateTime>(startOfRecurrence);

        destination.RecurrenceRules = CreateRecurrenceRule(source.RepeatNew.Value);

        static LocalDateTime? GetRecurrenceStart(GTDRepeatFrom repeatFrom, LocalDateTime? dueDate, LocalDateTime? completed, LocalDateTime? startDate, LocalDateTime created)
        {
            return repeatFrom == GTDRepeatFrom.FromDueDate ? dueDate ?? completed ?? startDate ?? created : completed ?? dueDate ?? startDate ?? created;
        }
    }

    private static void MapStatus(GTDTaskModel source, Todo destination, ResolutionContext context)
    {
        destination.Status = source.MapStatus(source.Completed != null);
        // need to set completed here after setting Status, since Status-Map would overwrite this
        destination.Completed = context.Mapper.Map<CalDateTime>(source.Completed);
    }

    private static void AddProperties(GTDTaskModel source, Todo destination)
    {
        if (source.HideUntil > 0)
        {
            var hideUntil = new CalDateTime(DateTimeOffset.FromUnixTimeMilliseconds(source.HideUntil).UtcDateTime, "UTC");
            destination.Properties.Add(new CalendarProperty(IntermediateFormatPropertyNames.HideUntil, hideUntil));
        }
        destination.AddProperty(IntermediateFormatPropertyNames.DueFloat, source.Floating.ToString().ToLowerInvariant());
        destination.AddProperty(IntermediateFormatPropertyNames.Starred, source.Starred.ToString().ToLowerInvariant());
    }

    private static Alarm? CreateAlarmFromReminder(long reminder)
    {
        if (reminder > 43200)
        {
            if (reminder % 1000 != 0)
            {
                throw new ArgumentException("Reminder must be a multiple of 1000");
            }
            var reminderDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(reminder);
            return new Alarm { Trigger = new Trigger { DateTime = new CalDateTime(reminderDateTime) } };
        }
        else if (reminder >= 0)
            return new Alarm { Trigger = new Trigger { Duration = Ical.Net.DataTypes.Duration.FromMinutes(-(int)reminder) } };

        return null;
    }

    private static List<RecurrencePattern> CreateRecurrenceRule(GTDRepeatInfoModel repeatInfo)
    {
        var freq = repeatInfo.Period switch
        {
            Period.Day => FrequencyType.Daily,
            Period.Week => FrequencyType.Weekly,
            Period.Month => FrequencyType.Monthly,
            Period.Year => FrequencyType.Yearly,
            _ => throw new ArgumentOutOfRangeException($"{repeatInfo.Period} not supported"),
        };

        return [new(freq, repeatInfo.Interval)];
    }
}
