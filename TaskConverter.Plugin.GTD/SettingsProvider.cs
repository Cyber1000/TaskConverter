using NodaTime;
using TaskConverter.Commons;

namespace TaskConverter.Plugin.GTD;

public class SettingsProvider(GTDConverterPlugin converterPlugin) : ISettingsProvider
{
    private readonly GTDConverterPlugin ConverterPlugin = converterPlugin ?? throw new Exception("Plugin must not be null");

    public DateTimeZone CurrentDateTimeZone =>
        DateTimeZoneProviders.Tzdb.GetZoneOrNull(ConverterPlugin!.ConversionAppData.GetAppSetting("TimeZoneId", "")) ?? DateTimeZoneProviders.Tzdb.GetSystemDefault();

    public bool AllowIncompleteMappingIfMoreThanOneItem => ConverterPlugin!.ConversionAppData.GetAppSetting("GTD.AllowIncompleteMappingIfMoreThanOneItem", false);
}
