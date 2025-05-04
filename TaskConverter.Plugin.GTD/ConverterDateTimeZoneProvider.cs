using NodaTime;

namespace TaskConverter.Plugin.GTD;

//TODO HH: move interface to base?
public interface IConverterDateTimeZoneProvider
{
    DateTimeZone CurrentDateTimeZone { get; }
}

public class ConverterDateTimeZoneProvider(GTDConverterPlugin converterPlugin) : IConverterDateTimeZoneProvider
{
    private readonly GTDConverterPlugin ConverterPlugin = converterPlugin ?? throw new Exception("Plugin must not be null");

    public DateTimeZone CurrentDateTimeZone =>
        DateTimeZoneProviders.Tzdb.GetZoneOrNull(ConverterPlugin!.ConversionAppData.GetAppSetting("TimeZoneId", "")) ?? DateTimeZoneProviders.Tzdb.GetSystemDefault();
}
