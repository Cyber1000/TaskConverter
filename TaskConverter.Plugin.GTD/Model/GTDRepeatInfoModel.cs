using System.Text.RegularExpressions;

namespace TaskConverter.Plugin.GTD.Model;

public readonly struct GTDRepeatInfoModel
{
    // Anchored on purpose. Unanchored, "Last day of every 3 months" matched the substring
    // "every 3 month" and was silently read as a plain three-month repetition, which is a
    // different recurrence than the one the app means.
    private const string IntervalPeriodPattern = @"^every (?<interval>\d+) (?<period>[^s]+)s?$";
    private const string PeriodlyPattern = @"^(?<period>[^s]+)ly$";
    private const string DailyPattern = @"^daily$";
    private const string BiPeriodPattern = @"^bi(?<period>[^s]+)ly$";
    private const string QuarterlyPattern = @"^quarterly$";
    private const string SemiannuallyPattern = @"^semiannually$";

    private static readonly Func<string, (bool Success, int Interval, Period Period)>[] searchFunctions =
    [
        (repeatInfo) => GetIntervalPeriod(repeatInfo, IntervalPeriodPattern),
        (repeatInfo) => GetIntervalPeriod(repeatInfo, PeriodlyPattern),
        (repeatInfo) => GetIntervalPeriod(repeatInfo, DailyPattern, 1, Period.Day),
        (repeatInfo) => GetIntervalPeriod(repeatInfo, BiPeriodPattern, 2),
        (repeatInfo) => GetIntervalPeriod(repeatInfo, QuarterlyPattern, 3, Period.Month),
        (repeatInfo) => GetIntervalPeriod(repeatInfo, SemiannuallyPattern, 6, Period.Month)
    ];

    private static readonly Dictionary<string, RepeatPattern> weekdaySetPatterns = new(StringComparer.OrdinalIgnoreCase)
    {
        { "BusinessDay", RepeatPattern.BusinessDay },
        { "Business Day", RepeatPattern.BusinessDay },
        { "Weekend", RepeatPattern.Weekend },
    };

    public int Interval { get; }

    public Period Period { get; }

    public RepeatPattern Pattern { get; }

    public GTDRepeatInfoModel(string repeatInfo)
    {
        ArgumentNullException.ThrowIfNull(repeatInfo);

        // Before the interval patterns: "Weekend" contains no interval and no period, and would
        // otherwise fall through to the exception.
        if (weekdaySetPatterns.TryGetValue(repeatInfo.Trim(), out var weekdaySetPattern))
        {
            Pattern = weekdaySetPattern;
            return;
        }

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

        throw new UnsupportedRepeatModeException(repeatInfo);
    }

    public GTDRepeatInfoModel(int interval, Period period)
    {
        Interval = interval;
        Period = period;
    }

    public GTDRepeatInfoModel(RepeatPattern pattern)
    {
        Pattern = pattern;
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
        if (Pattern != RepeatPattern.Interval)
            return Pattern.ToString();

        return $"Every {Interval} {GetPeriodText(Interval, Period)}";

        static string GetPeriodText(int interval, Period period)
        {
            var periodString = interval == 1 ? period.ToString() : $"{period}s";
            return periodString.ToLower();
        }
    }
}
