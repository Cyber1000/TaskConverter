using System.IO.Abstractions;
using NodaTime;
using TaskConverter.Commons;
using TaskConverter.Plugin.Base;
using TaskConverter.Plugin.GTD.Conversion;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD;

public class GTDConverterPlugin(IConversionAppSettings conversionAppSettings) : ConverterPluginBase<GTDDataModel>(conversionAppSettings)
{
    private readonly FileSystem _fileSystem = new();

    private readonly IJsonConfigurationSerializer _jsonConfigurationSerializer = new JsonConfigurationSerializer();

    protected override IConversionService<GTDDataModel> CreateConversionService(IConversionAppSettings conversionAppSettings)
    {
        var clock = SystemClock.Instance;
        var settingsProvider = new SettingsProvider(conversionAppSettings, Name);
        return new ConversionService(clock, settingsProvider, _fileSystem);
    }

    public override string Name => "GTD";

    protected override IReader<GTDDataModel?>? CreateReader() => new JsonConfigurationReader(_fileSystem, _jsonConfigurationSerializer);

    protected override IWriter<GTDDataModel?>? CreateWriter() => new JsonConfigurationWriter(_fileSystem, _jsonConfigurationSerializer);
}
