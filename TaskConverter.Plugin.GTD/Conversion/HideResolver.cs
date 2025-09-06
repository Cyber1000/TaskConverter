using AutoMapper;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using NodaTime;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Utils;
using Period = TaskConverter.Plugin.GTD.Model.Period;

namespace TaskConverter.Plugin.GTD.Conversion;

public class HideResolver(DateTimeZone dateTimeZone) : IValueResolver<Todo, GTDTaskModel, Hide>
{
    public enum HideBase
    {
        FromDueDate,
    }

    private readonly (int Interval, Period Period, (Hide Type, HideBase Base)[] BaseInfo)[] hideMapper = [(6, Period.Month, new[] { (Hide.SixMonthsBeforeDue, HideBase.FromDueDate) })];

    public Hide Resolve(Todo source, GTDTaskModel destination, Hide destMember, ResolutionContext context)
    {
        var dueDate = source.Due;
        var hideUntil = source.Properties.Get<CalDateTime>("X-HIDE-UNTIL");
        if (hideUntil == null)
            return Hide.DontHide;

        foreach (var hideMapperItem in hideMapper)
        {
            var period = hideMapperItem.Period switch
            {
                Period.Month => NodaTime.Period.FromMonths(-hideMapperItem.Interval),
                _ => throw new NotImplementedException($"Period {hideMapperItem.Period} not implemented"),
            };
            foreach (var baseInfo in hideMapperItem.BaseInfo)
            {
                if (baseInfo.Base != HideBase.FromDueDate)
                    throw new NotImplementedException($"Base {baseInfo.Base} not implemented");

                if (dueDate != null && dueDate.GetLocalDateTime(dateTimeZone).Plus(period) == hideUntil.GetLocalDateTime(dateTimeZone))
                {
                    return baseInfo.Type;
                }
            }
        }

        return Hide.GivenDate;
    }
}
