using Ical.Net.DataTypes;
using NodaTime;

namespace TaskConverter.Plugin.GTD.Utils
{
    public static class DateTimeExtensions
    {
        public static LocalDateTime GetLocalDateTime(this IDateTime dateTime, DateTimeZone timeZone)
        {
            return dateTime.GetInstant().InZone(timeZone).LocalDateTime;
        }

        public static Instant GetInstant(this IDateTime dateTime)
        {
            return Instant.FromDateTimeUtc(DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc));
        }

        public static IDateTime GetIDateTime(this LocalDateTime localDateTime, DateTimeZone timeZone)
        {
            return new CalDateTime(localDateTime.InZoneLeniently(timeZone).ToDateTimeUtc(), "UTC");
        }

        public static IDateTime? GetIDateTime(this LocalDateTime? localDateTime, DateTimeZone timeZone)
        {
            return localDateTime.HasValue ? localDateTime.Value.GetIDateTime(timeZone) : null;
        }

        public static IDateTime GetCurrentDateTime(DateTimeZone timeZone)
        {
            return new CalDateTime(SystemClock.Instance.GetCurrentInstant().InZone(timeZone).ToDateTimeUtc(), "UTC");
        }
    }
}
