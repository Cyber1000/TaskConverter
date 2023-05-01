using NodaTime;

namespace TaskConverter.Model.Mapper;

public interface IConverterDateTimeZoneProvider
{
    DateTimeZone CurrentDateTimeZone { get; }
}
