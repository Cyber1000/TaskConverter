using NodaTime;
using TaskConverter.Plugin.Base;

namespace TaskConverter.Plugin.Ical.Tests.Utils
{
    public class TestSettingsProvider : ISettingsProvider
    {
        public DateTimeZone CurrentDateTimeZone => throw new NotImplementedException();

        public T GetPluginSpecificSetting<T>(string settingName, T? defaultValue = default) => throw new NotImplementedException();
    }
}
