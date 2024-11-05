namespace TaskConverter.Model.Model;

public struct RepeatInfoModel(int interval, Period period, RepeatFrom repeatFrom)
{
    public int Interval { get; } = interval;

    public Period Period { get; } = period;

    public RepeatFrom RepeatFrom { get; set; } = repeatFrom;
}
