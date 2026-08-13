using TaskConverter.Plugin.Base;

namespace TaskConverter.Plugin.Ical;

public static class SettingsProviderExtensions
{
    /// <summary>
    /// Off by default: the destination may hold files this converter did not write, and removing
    /// something there is the user's decision.
    /// </summary>
    public static bool DeleteOrphanedFiles(this ISettingsProvider settingsProvider)
    {
        return settingsProvider.GetPluginSpecificSetting("DeleteOrphanedFiles", false);
    }
}
