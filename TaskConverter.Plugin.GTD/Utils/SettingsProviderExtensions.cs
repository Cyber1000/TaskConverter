using TaskConverter.Commons;

namespace TaskConverter.Plugin.GTD.Utils
{
    public static class SettingsProviderExtensions
    {
        public static bool AllowIncompleteMappingIfMoreThanOneItem(this ISettingsProvider settingsProvider)
        {
            return settingsProvider.GetPluginSpecificSetting("AllowIncompleteMappingIfMoreThanOneItem", false);
        }

        public static string GetPreferenceFilePath(this ISettingsProvider settingsProvider)
        {
            return settingsProvider.GetPluginSpecificSetting("PreferenceFilePath", "");
        }
    }
}