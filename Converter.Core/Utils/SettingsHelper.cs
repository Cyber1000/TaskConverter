using configurationManager = System.Configuration.ConfigurationManager;

namespace Converter.Core.Utils;

public static class SettingsHelper
{
    public static string GetAppSetting(string Key, string? defaultValue = null)
    {
        var appSettings = configurationManager.AppSettings;

        if (appSettings == null)
        {
            throw new Exception("Konnte Settings nicht laden");
        }

        var returnValue = appSettings.Get(Key);
        if (defaultValue == null && returnValue == null)
        {
            throw new Exception($@"Key ""{Key}"" nicht gefunden");
        }

        return returnValue ?? defaultValue ?? "";
    }
}
