using System.IO.Abstractions;
using Ical.Net;
using NodaTime;
using TaskConverter.Plugin.Base;
using TaskConverter.Plugin.GTD.Conversion;

namespace TaskConverter.Plugin.GTD;

public class GTDConverterPlugin : IConverterPlugin
{
    private JsonConfigurationReader? jsonReader = null;

    internal ConversionAppSettings ConversionAppData;
    private readonly FileSystem _fileSystem;
    private readonly ConversionService _conversionService;
    private readonly IJsonConfigurationSerializer _jsonConfigurationSerializer;

    public GTDConverterPlugin(ConversionAppSettings conversionAppData)
    {
        ConversionAppData = conversionAppData;
        _fileSystem = new FileSystem();
        _jsonConfigurationSerializer = new JsonConfigurationSerializer();
        var clock = SystemClock.Instance;
        var converterDateTimeZoneProvider = new ConverterDateTimeZoneProvider(this);
        _conversionService = new ConversionService(clock, converterDateTimeZoneProvider);
    }

    public string Name => "GTD";

    public bool SetLocation(string fromLocation)
    {
        try
        {
            jsonReader = new JsonConfigurationReader(_fileSystem.FileInfo.New(fromLocation), _fileSystem, _jsonConfigurationSerializer);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public (ConversionResult result, Exception? exception) CanConvertToIntermediateFormat()
    {
        if (jsonReader?.TaskInfo == null)
            return (ConversionResult.NoTasks, null);

        try
        {
            ConvertToIntermediateFormat();
            return (ConversionResult.CanConvert, null);
        }
        catch (Exception ex)
        {
            return (ConversionResult.ConversionError, ex);
        }
    }

    public Calendar? ConvertToIntermediateFormat()
    {
        return jsonReader?.TaskInfo == null ? null : _conversionService.MapToIntermediateFormat(jsonReader.TaskInfo);
    }

    public (bool isError, string validationError) ValidateSource()
    {
        return jsonReader?.ValidateRoundtrip() ?? (false, "");
    }
}
