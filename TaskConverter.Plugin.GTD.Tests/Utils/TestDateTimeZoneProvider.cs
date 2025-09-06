using NodaTime;
using TaskConverter.Commons;

namespace TaskConverter.Plugin.GTD.Tests.Utils;

public class TestDateTimeZoneProvider : IConverterDateTimeZoneProvider
{
    public DateTimeZone CurrentDateTimeZone => DateTimeZoneProviders.Tzdb.GetZoneOrNull("Europe/Vienna") ?? throw new ArgumentException("Europe/Vienna should exist");
}
