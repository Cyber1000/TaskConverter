using NodaTime;

namespace TaskConverter.Commons;

public interface ISettingsProvider
{
    DateTimeZone CurrentDateTimeZone { get; }
    T GetPluginSpecificSetting<T>(string settingName, T? defaultValue = default);
}
