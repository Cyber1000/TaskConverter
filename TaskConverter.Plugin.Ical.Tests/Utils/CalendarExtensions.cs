using Ical.Net;
using Ical.Net.Serialization;

namespace TaskConverter.Plugin.Ical.Tests.Utils
{
    public static class CalendarExtensions
    {
        public static string Serialize(this Calendar calendar)
        {
            var serializer = new CalendarSerializer();
            return serializer.SerializeToString(calendar) ?? string.Empty;
        }
    }
}