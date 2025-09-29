using Ical.Net.DataTypes;
using NodaTime;

namespace TaskConverter.Plugin.GTD.Utils
{
    public static class DateTimeExtensions
    {
        public static LocalDateTime GetLocalDateTime(this CalDateTime dateTime, DateTimeZone timeZone)
        {
            return dateTime.GetInstant().InZone(timeZone).LocalDateTime;
        }

        public static Instant GetInstant(this CalDateTime dateTime)
        {
            return Instant.FromDateTimeUtc(DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc));
        }

        public static CalDateTime GetCalDateTime(this LocalDateTime localDateTime, DateTimeZone timeZone)
        {
            return new CalDateTime(localDateTime.InZoneLeniently(timeZone).ToDateTimeUtc(), "UTC");
        }

        public static CalDateTime? GetCalDateTime(this LocalDateTime? localDateTime, DateTimeZone timeZone)
        {
            return localDateTime.HasValue ? localDateTime.Value.GetCalDateTime(timeZone) : null;
        }

        public static CalDateTime GetCurrentDateTime(DateTimeZone timeZone)
        {
            return new CalDateTime(SystemClock.Instance.GetCurrentInstant().InZone(timeZone).ToDateTimeUtc(), "UTC");
        }
    }
}
