using Ical.Net.DataTypes;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Utils;

/// <summary>
/// BusinessDay and Weekend are weekday sets, which RFC 5545 expresses exactly as a weekly
/// recurrence with BYDAY. Both directions read the sets from here so they cannot drift apart.
/// </summary>
public static class MapRepeatPatternExtension
{
    private static readonly Dictionary<RepeatPattern, DayOfWeek[]> daysByPattern = new()
    {
        { RepeatPattern.BusinessDay, [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday] },
        { RepeatPattern.Weekend, [DayOfWeek.Saturday, DayOfWeek.Sunday] },
    };

    public static List<WeekDay> ToByDay(this RepeatPattern pattern)
    {
        return daysByPattern.TryGetValue(pattern, out var days) ? days.Select(day => new WeekDay(day)).ToList() : [];
    }

    /// <summary>
    /// Returns <see cref="RepeatPattern.Interval"/> when the weekday set is not one of the two the
    /// app knows - a foreign client may write any combination, and that is an interval rule to us.
    /// </summary>
    public static RepeatPattern ToRepeatPattern(this IEnumerable<WeekDay>? byDay)
    {
        if (byDay is null)
            return RepeatPattern.Interval;

        var days = byDay.Select(weekDay => weekDay.DayOfWeek).ToHashSet();
        if (days.Count == 0)
            return RepeatPattern.Interval;

        return daysByPattern.FirstOrDefault(entry => days.SetEquals(entry.Value)).Key;
    }
}
