using Converter.Core.Mapper;
using Converter.Core.Utils;
using NodaTime;

namespace Converter.Console;

public class ConverterDateTimeZoneProvider : IConverterDateTimeZoneProvider
{
    public DateTimeZone CurrentDateTimeZone =>
        DateTimeZoneProviders.Tzdb.GetZoneOrNull(SettingsHelper.GetAppSetting("TimeZoneId", ""))
        ?? DateTimeZoneProviders.Tzdb.GetSystemDefault();
}
