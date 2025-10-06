using Ical.Net;
using TaskConverter.Commons;

namespace TaskConverter.Plugin.Base;

public abstract class ConverterPluginBase<T> : IConverterPlugin
    where T : class
{
    protected IReader<T?>? Reader { get; private set; } = null;

    public abstract string Name { get; }

    private readonly IConversionService<T> _conversionService;

    public ConverterPluginBase(IConversionAppSettings conversionAppSettings)
    {
        _conversionService = CreateConversionService(conversionAppSettings);
    }

    protected abstract IConversionService<T> CreateConversionService(IConversionAppSettings conversionAppSettings);

    public (ConversionResult result, Exception? exception) CanConvertToIntermediateFormat()
    {
        if (Reader?.Result == null)
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

    public Calendar? ConvertToIntermediateFormat() => Reader?.Result == null ? null : _conversionService.MapToIntermediateFormat(Reader.Result);

    public (bool isError, string validationError) CheckSource() => Reader?.CheckSource() ?? (false, "");

    public bool SetLocation(string fromLocation)
    {
        try
        {
            Reader = CreateReader(fromLocation);
            return true;
        }
        catch
        {
            return false;
        }
    }

    protected abstract IReader<T?>? CreateReader(string fromLocation);
}
