using NodaTime;

namespace Converter.Model.Mapper;

public interface IConverterDateTimeZoneProvider
{
    DateTimeZone CurrentDateTimeZone { get; }
}
