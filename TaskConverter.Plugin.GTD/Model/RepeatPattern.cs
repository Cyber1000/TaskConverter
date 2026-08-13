namespace TaskConverter.Plugin.GTD.Model;

/// <summary>
/// Most repeat modes are an interval over a period. Two are not: they name a set of weekdays.
/// </summary>
public enum RepeatPattern
{
    Interval,
    BusinessDay,
    Weekend,
}
