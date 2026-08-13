using System.IO.Abstractions;
using Ical.Net;
using Ical.Net.Serialization;
using TaskConverter.Commons;
using TaskConverter.Plugin.Base;

namespace TaskConverter.Plugin.Ical;

public class IcalConverterPlugin(IConversionAppSettings conversionAppSettings) : ConverterPluginBase<List<Calendar>>(conversionAppSettings)
{
    private readonly FileSystem _fileSystem = new();
    private readonly CalendarSerializer _calendarSerializer = new();
    private ISettingsProvider _settingsProvider = null!;

    protected override IConversionService<List<Calendar>> CreateConversionService(IConversionAppSettings conversionAppSettings)
    {
        _settingsProvider = new SettingsProvider(conversionAppSettings, Name);
        return new ConversionService(_settingsProvider);
    }

    public override string Name => "Ical";

    protected override IReader<List<Calendar>?>? CreateReader() => new IcalReader(_fileSystem);

    protected override IWriter<List<Calendar>?>? CreateWriter() => new IcalWriter(_fileSystem, _calendarSerializer, _settingsProvider);
}
