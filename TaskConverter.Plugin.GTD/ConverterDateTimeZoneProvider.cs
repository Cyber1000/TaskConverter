using NodaTime;
using TaskConverter.Commons;

namespace TaskConverter.Plugin.GTD;

public class ConverterDateTimeZoneProvider(GTDConverterPlugin converterPlugin) : IConverterDateTimeZoneProvider
{
    private readonly GTDConverterPlugin ConverterPlugin = converterPlugin ?? throw new Exception("Plugin must not be null");

    //TODO HH: should this be a GTD-specific setting? if not should ConverterDateTimeZoneProvider also be a global method?
    public DateTimeZone CurrentDateTimeZone =>
        DateTimeZoneProviders.Tzdb.GetZoneOrNull(ConverterPlugin!.ConversionAppData.GetAppSetting("TimeZoneId", "")) ?? DateTimeZoneProviders.Tzdb.GetSystemDefault();
}
