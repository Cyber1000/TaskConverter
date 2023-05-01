using NodaTime;

namespace TaskConverter.Tests.Extensions;

public static class DateTimeExtensions
{
    public static LocalDateTime GetLocalDateTime(this Instant instant, DateTimeZone currentTimeZone)
    {
        return instant.InZone(currentTimeZone).LocalDateTime;
    }
}
