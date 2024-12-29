using System.Text.RegularExpressions;
using TaskConverter.Model.Model;

namespace TaskConverter.Plugin.GTD.Model;

public readonly struct GTDRepeatInfoModel
{
    private const string IntervalPeriodPattern = @"every (?<interval>\d+) (?<period>[^s]*)";
    private const string PeriodlyPattern = @"(?<period>[^s]+)ly";
    private const string DailyPattern = @"daily";
    private const string BiPeriodPattern = @"bi(?<period>[^s]+)ly";
    private const string QuarterlyPattern = @"quarterly";
    private const string SemiannuallyPattern = @"semiannually";

    private static readonly Func<string, (bool Success, int Interval, Period Period)>[] searchFunctions =
    [
        (repeatInfo) => GetIntervalPeriod(repeatInfo, IntervalPeriodPattern),
        (repeatInfo) => GetIntervalPeriod(repeatInfo, PeriodlyPattern),
        (repeatInfo) => GetIntervalPeriod(repeatInfo, DailyPattern, 1, Period.Day),
        (repeatInfo) => GetIntervalPeriod(repeatInfo, BiPeriodPattern, 2),
        (repeatInfo) => GetIntervalPeriod(repeatInfo, QuarterlyPattern, 3, Period.Month),
        (repeatInfo) => GetIntervalPeriod(repeatInfo, SemiannuallyPattern, 6, Period.Month)
    ];

    public int Interval { get; }

    public Period Period { get; }

    public GTDRepeatInfoModel(string repeatInfo)
    {
        ArgumentNullException.ThrowIfNull(repeatInfo);
        foreach (var searchFunction in searchFunctions)
        {
            var (success, interval, period) = searchFunction.Invoke(repeatInfo);
            if (success)
            {
                Interval = interval;
                Period = period;
                return;
            }
        }

        throw new NotImplementedException($"Cannot cast \"{repeatInfo}\" to RepeatInfo");
    }

    public GTDRepeatInfoModel(int interval, Period period)
    {
        Interval = interval;
        Period = period;
    }

    private static (bool Success, int Interval, Period Period) GetIntervalPeriod(
        string repeatInfo,
        string searchPattern,
        int? setInterval = null,
        Period? setPeriod = null
    )
    {
        var match = Regex.Match(repeatInfo, searchPattern, RegexOptions.IgnoreCase);
        var interval = setInterval ?? 1;
        var period = setPeriod ?? Period.Day;
        if (match.Success)
        {
            if (match.Groups["interval"].Success)
            {
                if (!int.TryParse(match.Groups["interval"].Value, out interval))
                {
                    return (false, interval, period);
                }
            }
            if (match.Groups["period"].Success)
            {
                if (!Enum.TryParse(typeof(Period), match.Groups["period"].Value, true, out var periodObject))
                {
                    return (false, interval, period);
                }
                period = (Period)periodObject;
            }
        }
        return (match.Success, interval, period);
    }

    public override string ToString()
    {
        return $"Every {Interval} {GetPeriodText(Interval, Period)}";

        static string GetPeriodText(int interval, Period period)
        {
            var periodString = interval == 1 ? period.ToString() : $"{period}s";
            return periodString.ToLower();
        }
    }
}
