using Ical.Net;
using TaskConverter.Commons;

namespace TaskConverter.Plugin.Base;

public abstract class ConverterPluginBase<T> : IConverterPlugin
    where T : class
{
    public abstract string Name { get; }

    private IReader<T?>? Reader { get; set; } = null;
    private IWriter<T?>? Writer { get; set; } = null;

    private readonly IConversionService<T> _conversionService;

    public ConverterPluginBase(IConversionAppSettings conversionAppSettings)
    {
        _conversionService = CreateConversionService(conversionAppSettings);
    }

    protected abstract IConversionService<T> CreateConversionService(IConversionAppSettings conversionAppSettings);

    public CanConvertResult CanConvertToIntermediateFormat(string source)
    {
        var conversionResult = ConvertToIntermediateFormat(source);
        return new CanConvertResult(conversionResult.Success, conversionResult.ResultType, conversionResult.Exception);
    }

    public ConvertToResult ConvertToIntermediateFormat(string source)
    {
        if (!EnsureReaderIsSet())
            return new ConvertToResult(false, ConversionResultType.ReaderError, null, null);

        var readerResult = Reader!.Read(source);
        if (readerResult == null)
            return new ConvertToResult(false, ConversionResultType.NoTasks, null, null);

        try
        {
            var calendar = _conversionService.MapToIntermediateFormat(readerResult);
            return new ConvertToResult(true, ConversionResultType.CanConvert, calendar, null);
        }
        catch (Exception ex)
        {
            return new ConvertToResult(false, ConversionResultType.ConversionError, null, ex);
        }
    }

    public SourceResult CheckSource(string source)
    {
        if (!EnsureReaderIsSet())
            return new SourceResult(false, new Exception("Reader could not be set."));

        return Reader!.CheckSource(source);
    }

    public ConvertFromResult ConvertFromIntermediateFormat(string destination, Calendar sourceModel)
    {
        if (!EnsureWriterIsSet())
            return new ConvertFromResult(false, ConversionResultType.WriterError, new Exception("Reader could not be set."));

        var destinationModel = _conversionService.MapFromIntermediateFormat(sourceModel);
        try
        {
            Writer!.Write(destination, destinationModel);
            return new ConvertFromResult(true, ConversionResultType.CanConvert, null);
        }
        catch(Exception ex)
        {
            return new ConvertFromResult(false, ConversionResultType.ConversionError, ex);
        }
    }

    private bool EnsureReaderIsSet()
    {
        try
        {
            Reader = CreateReader();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private bool EnsureWriterIsSet()
    {
        try
        {
            Writer = CreateWriter();
            return true;
        }
        catch
        {
            return false;
        }
    }

    protected abstract IReader<T?>? CreateReader();
    protected abstract IWriter<T?>? CreateWriter();
}
