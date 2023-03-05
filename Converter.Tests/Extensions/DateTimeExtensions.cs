using NodaTime;

namespace Converter.Tests.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly DateTimeZone currentTimeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();

        public static LocalDateTime GetLocalDateTime(this Instant instant)
        {
            return instant.InZone(currentTimeZone).LocalDateTime;
        }
    }
}
