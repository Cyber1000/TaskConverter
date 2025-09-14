using AutoMapper;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using TaskConverter.Plugin.GTD.Model;
using Period = TaskConverter.Plugin.GTD.Model.Period;

namespace TaskConverter.Plugin.GTD.Conversion;

public class AfterMapTodoFromIntermediateFormat : IMappingAction<Todo, GTDTaskModel>
{
    public void Process(Todo source, GTDTaskModel destination, ResolutionContext context)
    {
        MapHide(source, destination);
        MapFloatingDate(source, destination);
        MapStarred(source, destination);
        MapRepetition(source, destination, context);
    }

    private static void MapFloatingDate(Todo source, GTDTaskModel destination)
    {
        if (bool.TryParse(source.Properties.Get<string>(IntermediateFormatPropertyNames.DueFloat), out var floating) && floating)
        {
            destination.Floating = floating;
            destination.DueDateModifier = DueDateModifier.OptionallyOn;
        }
    }

    private static void MapStarred(Todo source, GTDTaskModel destination)
    {
        if (bool.TryParse(source.Properties.Get<string>(IntermediateFormatPropertyNames.Starred), out var starred) && starred)
        {
            destination.Starred = starred;
        }
    }

    private static void MapRepetition(Todo source, GTDTaskModel destination, ResolutionContext context)
    {
        var settingsProvider = context.GetSettingsProvider();

        destination.RepeatFrom = source.Start?.Equals(source.Due) ?? true ? GTDRepeatFrom.FromDueDate : GTDRepeatFrom.FromCompletion;
        if (source.RecurrenceRules?.Count > 1)
        {
            if (settingsProvider.AllowIncompleteMappingIfMoreThanOneItem)
                Console.WriteLine("More than one RecurrenceRule, can only convert the first.");
            else
                throw new Exception("More than one RecurrenceRule. This is only allowed if AllowIncompleteMappingIfMoreThanOneItem is true.");
        }
        destination.RepeatNew = CreateGTDRepeatInfoModel(source.RecurrenceRules?.FirstOrDefault());
    }

    private static void MapHide(Todo source, GTDTaskModel destination)
    {
        var hideUntil = source.Properties.Get<CalDateTime>(IntermediateFormatPropertyNames.HideUntil);
        if (hideUntil != null)
            destination.HideUntil = new DateTimeOffset(hideUntil.Value).ToUnixTimeMilliseconds();
    }

    private static GTDRepeatInfoModel? CreateGTDRepeatInfoModel(RecurrencePattern? recurrencePattern)
    {
        if (recurrencePattern == null)
            return null;

        var period = recurrencePattern.Frequency switch
        {
            FrequencyType.Daily => Period.Day,
            FrequencyType.Weekly => Period.Week,
            FrequencyType.Monthly => Period.Month,
            FrequencyType.Yearly => Period.Year,
            _ => throw new ArgumentOutOfRangeException($"{recurrencePattern.Frequency} not supported"),
        };

        return new GTDRepeatInfoModel(recurrencePattern.Interval, period);
    }
}
