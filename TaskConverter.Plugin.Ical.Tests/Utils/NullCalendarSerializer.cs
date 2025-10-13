using Ical.Net.Serialization;

namespace TaskConverter.Plugin.Ical.Tests.Utils;

public class NullCalendarSerializer : CalendarSerializer
{
    public override string? SerializeToString(object? obj) => null;
}