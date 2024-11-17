using TaskConverter.Model.Model;
using TaskConverter.Plugin.Base;
using NodaTime;

namespace TaskConverter.Plugin.GTD;

public class GTDConverterPlugin : IConverterPlugin
{
    private JsonConfigurationReader? jsonReader = null;

    internal ConversionAppSettings ConversionAppSettings;

    private readonly Mapper.Converter converter;

    public GTDConverterPlugin(params object[] args)
    {
        ConversionAppSettings = (ConversionAppSettings)args[0];
        var clock = SystemClock.Instance;
        var converterDateTimeZoneProvider = new ConverterDateTimeZoneProvider(this);
        converter = new Mapper.Converter(clock, converterDateTimeZoneProvider);
    }

    public string Name => "GTD";

    public bool SetLocation(string fromLocation)
    {
        try
        {
            jsonReader = new JsonConfigurationReader(new FileInfo(fromLocation));
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
