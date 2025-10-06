using NodaTime;
using TaskConverter.Commons;

namespace TaskConverter.Plugin.Base;

public class SettingsProvider(IConversionAppSettings conversionAppSettings, string pluginName) : ISettingsProvider
{
    private readonly IConversionAppSettings ConversionAppSettings = conversionAppSettings ?? throw new Exception("Plugin must not be null");

    //TODO HH: gtd-specific?
    public DateTimeZone CurrentDateTimeZone =>
        DateTimeZoneProviders.Tzdb.GetZoneOrNull(ConversionAppSettings.GetAppSetting("TimeZoneId", "")) ?? DateTimeZoneProviders.Tzdb.GetSystemDefault();

    public T GetPluginSpecificSetting<T>(string settingName, T? defaultValue = default)
    {
        return ConversionAppSettings.GetAppSetting($"{pluginName}.{settingName}", defaultValue);
    }
}
