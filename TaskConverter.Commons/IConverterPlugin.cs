using Ical.Net;

namespace TaskConverter.Commons;

public interface IConverterPlugin
{
    string Name { get; }

    //TODO HH: only fromLocation?
    bool SetLocation(string fromLocation);

    (ConversionResult result, Exception? exception) CanConvertToIntermediateFormat();

    Calendar? ConvertToIntermediateFormat();

    public (bool isError, string validationError) ValidateSource();
}
