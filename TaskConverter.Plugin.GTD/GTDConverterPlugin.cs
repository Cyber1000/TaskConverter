using System.IO.Abstractions;
using NodaTime;
using TaskConverter.Model.Model;
using TaskConverter.Plugin.Base;

namespace TaskConverter.Plugin.GTD;

public class GTDConverterPlugin : IConverterPlugin
{
    private JsonConfigurationReader? jsonReader = null;

    internal ConversionAppSettings ConversionAppData;

    private readonly Mapper.Converter converter;

    public GTDConverterPlugin(ConversionAppSettings conversionAppData)
    {
        ConversionAppData = conversionAppData;
        var clock = SystemClock.Instance;
        var converterDateTimeZoneProvider = new ConverterDateTimeZoneProvider(this);
        converter = new Mapper.Converter(clock, converterDateTimeZoneProvider);
    }

    public string Name => "GTD";

    public bool SetLocation(string fromLocation)
    {
        try
        {
            var fileSystem = new FileSystem();
            jsonReader = new JsonConfigurationReader(fileSystem.FileInfo.New(fromLocation), fileSystem);
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
        return jsonReader?.Validate() ?? (false, "");
    }
}
