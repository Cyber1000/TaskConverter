using TaskConverter.Commons;
using TaskConverter.Plugin.GTD.TodoModel;

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

        public static string GetIntermediateFormatSymbol(this ISettingsProvider settingsProvider, KeyWordType keyWordType)
        {
            return settingsProvider.GetPluginSpecificSetting($"IntermediateFormat.Symbol.{keyWordType}", "");
        }

        public static string GetGTDFormatSymbol(this ISettingsProvider settingsProvider, KeyWordType keyWordType)
        {
            return settingsProvider.GetPluginSpecificSetting($"GTDFormat.Symbol.{keyWordType}", "");
        }
    }
}