using NodaTime;

namespace TaskConverter.Plugin.Base;

public interface ISettingsProvider
{
    DateTimeZone CurrentDateTimeZone { get; }
    T GetPluginSpecificSetting<T>(string settingName, T? defaultValue = default);
}
