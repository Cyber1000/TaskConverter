using AutoMapper;
using Converter.Core.GTD.InternalModel;
using Converter.Core.GTD.Model;
using NodaTime;

namespace Converter.Core.Mapper;

public class HideResolver : IValueResolver<TaskModel, TaskInfoTaskEntry, Hide>
{
    public enum HideBase
    {
        FromDueDate
    }

    private (int Interval, GTD.Model.Period Period, (Hide Type, HideBase Base)[] BaseInfo)[] hideMapper =
    {
        (6, GTD.Model.Period.Month, new[] { (Hide.SixMonthsBeforeDue, HideBase.FromDueDate) })
    };

    public DateTimeZone DateTimeZone { get; }

    public HideResolver(DateTimeZone dateTimeZone)
    {
        DateTimeZone = dateTimeZone;
    }

    public Hide Resolve(TaskModel source, TaskInfoTaskEntry destination, Hide destMember, ResolutionContext context)
    {
        var dueDate = source.DueDate;
        var hideUntil = source.HideUntil;

        foreach (var hideMapperItem in hideMapper)
        {
            var period = hideMapperItem.Period switch
            {
                GTD.Model.Period.Month => NodaTime.Period.FromMonths(-hideMapperItem.Interval),
                _ => throw new NotImplementedException($"Period {hideMapperItem.Period} not implemented"),
            };
            foreach (var baseInfo in hideMapperItem.BaseInfo)
            {
                if (baseInfo.Base != HideBase.FromDueDate)
                    throw new NotImplementedException($"Base {baseInfo.Base} not implemented");

                if (dueDate.HasValue && dueDate.Value.Plus(period).InZoneLeniently(DateTimeZone).ToInstant() == hideUntil)
                {
                    return baseInfo.Type;
                }
            }
        }

        return hideUntil == null ? Hide.DontHide : Hide.GivenDate;
    }
}
