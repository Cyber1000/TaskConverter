using System.IO.Abstractions;
using Ical.Net;
using NodaTime;
using TaskConverter.Plugin.Base;

namespace TaskConverter.Plugin.GTD;

//TODO HH: TaskConverter.Plugin.GTD compiled twice
public class GTDConverterPlugin : IConverterPlugin
{
    private JsonConfigurationReader? jsonReader = null;

    internal ConversionAppSettings ConversionAppData;
    private readonly FileSystem _fileSystem;
    private readonly Mapper.Converter converter;
    private readonly IJsonConfigurationSerializer _jsonConfigurationSerializer;

    public GTDConverterPlugin(ConversionAppSettings conversionAppData)
    {
        ConversionAppData = conversionAppData;
        _fileSystem = new FileSystem();
        _jsonConfigurationSerializer = new JsonConfigurationSerializer();
        var clock = SystemClock.Instance;
        var converterDateTimeZoneProvider = new ConverterDateTimeZoneProvider(this);
        converter = new Mapper.Converter(clock, converterDateTimeZoneProvider);
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

    public (ConversionResult result, Exception? exception) CanConvertToCalendar()
    {
        if (jsonReader?.TaskInfo == null)
            return (ConversionResult.NoTasks, null);

        try
        {
            ConvertToCalendar();
            return (ConversionResult.CanConvert, null);
        }
        catch (Exception ex)
        {
            return (ConversionResult.ConversionError, ex);
        }
    }

    public Calendar? ConvertToCalendar()
    {
        return jsonReader?.TaskInfo == null ? null : converter.MapToModel(jsonReader.TaskInfo);
    }

    public (bool isError, string validationError) ValidateSource()
    {
        return jsonReader?.ValidateRoundtrip() ?? (false, "");
    }
}
