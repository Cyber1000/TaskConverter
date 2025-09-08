using AutoMapper;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Utils;
using Period = TaskConverter.Plugin.GTD.Model.Period;

namespace TaskConverter.Plugin.GTD.Conversion;

public class AfterMapTodoToIntermediateFormat : IMappingAction<GTDTaskModel, Todo>
{
    public void Process(GTDTaskModel source, Todo destination, ResolutionContext context)
    {
        MapAlarm(source, destination);
        MapRecurrenceRule(source, destination);
        MapDates(source, destination, context);
        AddProperties(source, destination);
    }

    private static void MapAlarm(GTDTaskModel source, Todo destination)
    {
        var alarm = CreateAlarmFromReminder(source.Reminder);
        if (alarm != null)
            destination.Alarms.Add(alarm);
    }

    private static void MapRecurrenceRule(GTDTaskModel source, Todo destination)
    {
        destination.RecurrenceRules = CreateRecurrenceRule(source.RepeatNew);
    }

    private static void MapDates(GTDTaskModel source, Todo destination, ResolutionContext context)
    {
        //TODO HH: not really exact, since Completed may be null at the time of mapping
        var start = source.RepeatFrom == GTDRepeatFrom.FromDueDate ? source.DueDate : source.Completed;
        destination.Start = start.HasValue && source.RepeatNew != null ? context.Mapper.Map<IDateTime>(start.Value) : null;

        //TODO HH: state should be completed if this is set
        destination.Completed = source.Completed.GetIDateTime(context.GetTimeZone()); // need to set this here, since Status-Map would overwrite this
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
            var reminderDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(reminder);
            return new Alarm { Trigger = new Trigger { DateTime = new CalDateTime { Value = reminderDateTime } } };
        }
        else if (reminder >= 0)
            return new Alarm { Trigger = new Trigger { Duration = TimeSpan.FromMinutes(-reminder) } };

        return null;
    }

    private static List<RecurrencePattern>? CreateRecurrenceRule(GTDRepeatInfoModel? repeatInfo)
    {
        if (!repeatInfo.HasValue)
            return null;

        var freq = repeatInfo.Value.Period switch
        {
            Period.Day => FrequencyType.Daily,
            Period.Week => FrequencyType.Weekly,
            Period.Month => FrequencyType.Monthly,
            Period.Year => FrequencyType.Yearly,
            _ => throw new ArgumentOutOfRangeException($"{repeatInfo.Value.Period} not supported"),
        };

        return [new(freq, repeatInfo.Value.Interval)];
    }
}