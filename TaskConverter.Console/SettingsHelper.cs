using TaskConverter.Plugin.Base;
using configurationManager = System.Configuration.ConfigurationManager;

namespace TaskConverter.Console;

static class SettingsHelper
{
    public static Dictionary<string, string> GetAppSettings()
    {
        var appSettings = configurationManager.AppSettings ?? throw new Exception("Couldn't load settings");

        return appSettings.AllKeys.ToDictionary(k => k ?? "", k => appSettings[k] ?? "");
    }
}
