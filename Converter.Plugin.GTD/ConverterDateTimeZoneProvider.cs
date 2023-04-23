using Converter.Model.Mapper;
using NodaTime;

namespace Converter.Plugin.GTD;

public class ConverterDateTimeZoneProvider : IConverterDateTimeZoneProvider
{
    private readonly GTDConverterPlugin ConverterPlugin;

    public ConverterDateTimeZoneProvider(GTDConverterPlugin converterPlugin)
    {
        ConverterPlugin = converterPlugin ?? throw new Exception("Plugin must not be null");
    }

    public DateTimeZone CurrentDateTimeZone =>
        DateTimeZoneProviders.Tzdb.GetZoneOrNull(ConverterPlugin!.ConversionAppSettings.GetAppSetting("TimeZoneId", ""))
        ?? DateTimeZoneProviders.Tzdb.GetSystemDefault();
}
