using NodaTime;
using TaskConverter.Commons;

namespace TaskConverter.Plugin.GTD.Tests.Utils;

public class TestSettingsProvider : ISettingsProvider
{

    public DateTimeZone CurrentDateTimeZone => DateTimeZoneProviders.Tzdb.GetZoneOrNull("Europe/Vienna") ?? throw new ArgumentException("Europe/Vienna should exist");

    public bool AllowIncompleteMappingIfMoreThanOneItem { get; set; }

    public string PreferenceFilePath { get; set; } = "";

    public T GetPluginSpecificSetting<T>(string settingName, T? defaultValue = default)
    {
        switch (settingName)
        {
            case "AllowIncompleteMappingIfMoreThanOneItem":
                {
                    object value = AllowIncompleteMappingIfMoreThanOneItem;
                    return (T)Convert.ChangeType(value, typeof(T));
                }

            case "PreferenceFilePath":
                {
                    object value = PreferenceFilePath;
                    return (T)Convert.ChangeType(value, typeof(T));
                }
        }

        throw new InvalidOperationException($"Unknown settingName {settingName}");
    }
}
