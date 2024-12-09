using NodaTime;

namespace TaskConverter.Tests.Utils;

public static class DateTimeExtensions
{
    public static LocalDateTime GetLocalDateTime(this Instant instant, DateTimeZone currentTimeZone)
    {
        return instant.InZone(currentTimeZone).LocalDateTime;
    }
}
