using NodaTime;
using TaskConverter.Commons;

namespace TaskConverter.Plugin.GTD.Tests.Utils;

public class TestSettingsProvider : ISettingsProvider
{

    public DateTimeZone CurrentDateTimeZone => DateTimeZoneProviders.Tzdb.GetZoneOrNull("Europe/Vienna") ?? throw new ArgumentException("Europe/Vienna should exist");

    public bool AllowIncompleteMappingIfMoreThanOneItem { get; set; }
}
