using NodaTime;
using TaskConverter.Plugin.Base;
using TaskConverter.Plugin.GTD.TodoModel;

namespace TaskConverter.Plugin.GTD.Tests.Utils;

public class TestSettingsProvider : ISettingsProvider
{
    public TestSettingsProvider()
    {
        SetIntermediateFormatSymbol(KeyWordType.Folder, "+");
        SetIntermediateFormatSymbol(KeyWordType.Context, "@");
        SetIntermediateFormatSymbol(KeyWordType.Status, "#");
        SetGTDFormatSymbol(KeyWordType.Folder, "+");
        SetGTDFormatSymbol(KeyWordType.Context, "@");
        SetGTDFormatSymbol(KeyWordType.Status, "#");
    }

    public Dictionary<KeyWordType, string> intermediateFormatSymbol = [];

    public Dictionary<KeyWordType, string> gtdFormatSymbol = [];

    public DateTimeZone CurrentDateTimeZone => DateTimeZoneProviders.Tzdb.GetZoneOrNull("Europe/Vienna") ?? throw new ArgumentException("Europe/Vienna should exist");

    public bool AllowIncompleteMappingIfMoreThanOneItem { get; set; }

    public string PreferenceFilePath { get; set; } = "";

    public void SetIntermediateFormatSymbol(KeyWordType keyWordType, string symbol)
    {
        intermediateFormatSymbol[keyWordType] = symbol;
    }

    public void SetGTDFormatSymbol(KeyWordType keyWordType, string symbol)
    {
        gtdFormatSymbol[keyWordType] = symbol;
    }

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
        var intermediateSettingName = "IntermediateFormat.Symbol.";
        if (settingName.StartsWith(intermediateSettingName) && Enum.TryParse<KeyWordType>(settingName[intermediateSettingName.Length..], out var intermediateFormatKeyWordType))
        {
            if (intermediateFormatSymbol.TryGetValue(intermediateFormatKeyWordType, out var intermediateValue))
                return (T)Convert.ChangeType(intermediateValue, typeof(T));
            else if (defaultValue != null)
                return (T)Convert.ChangeType(defaultValue, typeof(T));
        }
        var gtdSettingName = "GTDFormat.Symbol.";
        if (settingName.StartsWith(gtdSettingName) && Enum.TryParse<KeyWordType>(settingName[gtdSettingName.Length..], out var gtdFormatKeyWordType))
        {
            if (gtdFormatSymbol.TryGetValue(gtdFormatKeyWordType, out var gtdValue))
                return (T)Convert.ChangeType(gtdValue, typeof(T));
            else if (defaultValue != null)
                return (T)Convert.ChangeType(defaultValue, typeof(T));
        }

        throw new InvalidOperationException($"Unknown settingName {settingName}");
    }
}
