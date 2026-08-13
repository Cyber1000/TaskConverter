using NodaTime;
using TaskConverter.Plugin.Base;

namespace TaskConverter.Plugin.Ical.Tests.Utils
{
    public class TestSettingsProvider : ISettingsProvider
    {
        public bool DeleteOrphanedFiles { get; set; }

        public DateTimeZone CurrentDateTimeZone => throw new NotImplementedException();

        public T GetPluginSpecificSetting<T>(string settingName, T? defaultValue = default)
        {
            return settingName switch
            {
                "DeleteOrphanedFiles" => (T)Convert.ChangeType(DeleteOrphanedFiles, typeof(T)),
                _ => throw new InvalidOperationException($"Unknown settingName {settingName}"),
            };
        }
    }
}
