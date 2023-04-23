using Converter.Model.Mapper;
using NodaTime;

namespace Converter.Tests;

public class TestDateTimeZoneProvider : IConverterDateTimeZoneProvider
{
    public DateTimeZone CurrentDateTimeZone =>
        DateTimeZoneProviders.Tzdb.GetZoneOrNull("Europe/Vienna") ?? throw new ArgumentException("Europe/Vienna should exist");
}
