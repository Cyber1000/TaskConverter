using Ical.Net;

namespace TaskConverter.Commons;

public record CanConvertResult(bool Success, ConversionResultType ResultType, Exception? Exception);

public record ConvertToResult(bool Success, ConversionResultType ResultType, Calendar? Result, Exception? Exception);

public record ConvertFromResult(bool Success, ConversionResultType ResultType, Exception? Exception);

public record SourceResult(bool Success, Exception? Exception);

public interface IConverterPlugin
{
    string Name { get; }

    CanConvertResult CanConvertToIntermediateFormat(string source);

    ConvertToResult ConvertToIntermediateFormat(string source);

    SourceResult CheckSource(string source);

    ConvertFromResult ConvertFromIntermediateFormat(string destination, Calendar sourceModel);
}
