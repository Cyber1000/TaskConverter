using Ical.Net;

namespace TaskConverter.Plugin.Base;

public interface IConverterPlugin
{
    string Name { get; }

    bool SetLocation(string fromLocation);

    (ConversionResult result, Exception? exception) CanConvertToCalendar();

    Calendar? ConvertToCalendar();

    public (bool isError, string validationError) ValidateSource();
}
