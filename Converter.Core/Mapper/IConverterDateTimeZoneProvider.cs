using NodaTime;

namespace Converter.Core.Mapper;

public interface IConverterDateTimeZoneProvider
{
    DateTimeZone CurrentDateTimeZone { get; }
}
