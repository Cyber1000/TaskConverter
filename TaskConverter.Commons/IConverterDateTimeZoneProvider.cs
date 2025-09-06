using NodaTime;

namespace TaskConverter.Commons;

public interface IConverterDateTimeZoneProvider
{
    DateTimeZone CurrentDateTimeZone { get; }
}
