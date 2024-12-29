using System.IO.Abstractions;
using NodaTime;
using TaskConverter.Model.Model;
using TaskConverter.Plugin.Base;

namespace TaskConverter.Plugin.GTD;

public class GTDConverterPlugin : IConverterPlugin
{
    private JsonConfigurationReader? jsonReader = null;

    internal ConversionAppSettings ConversionAppData;
    private readonly IFileSystem _fileSystem;
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

    public (bool?, Exception? exception) CanConvertToTaskAppDataModel()
    {
        try
        {
            if (jsonReader?.TaskInfo == null)
                return (null, null);

            ConvertToTaskAppDataModel();
            return (true, null);
        }
        catch (Exception exception)
        {
            return (false, exception);
        }
    }

    public TaskAppDataModel? ConvertToTaskAppDataModel()
    {
        return jsonReader?.TaskInfo == null ? null : converter.MapToModel(jsonReader.TaskInfo);
    }

    public (bool isError, string validationError) ValidateSource()
    {
        return jsonReader?.ValidateRoundtrip() ?? (false, "");
    }
}
