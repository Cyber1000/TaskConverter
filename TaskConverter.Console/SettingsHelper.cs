using configurationManager = System.Configuration.ConfigurationManager;

namespace TaskConverter.Console;

static class SettingsHelper
{
    public static ConversionAppSettings GetAppSettings()
    {
        var appSettings = configurationManager.AppSettings ?? throw new Exception("Couldn't load settings");

        return new ConversionAppSettings(appSettings.AllKeys.ToDictionary(k => k ?? "", k => appSettings[k] ?? ""));
    }
}
