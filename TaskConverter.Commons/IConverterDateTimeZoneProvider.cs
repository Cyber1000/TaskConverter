using NodaTime;

namespace TaskConverter.Commons;

public interface ISettingsProvider
{
    DateTimeZone CurrentDateTimeZone { get; }
    bool AllowIncompleteMappingIfMoreThanOneItem { get; }
}
