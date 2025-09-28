using AutoMapper;
using Ical.Net;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Utils;

namespace TaskConverter.Plugin.GTD.Conversion;

public class HandlePreferencesToIntermediateFormat : IMappingAction<GTDDataModel, Calendar>
{
    public void Process(GTDDataModel source, Calendar destination, ResolutionContext context)
    {
        var settingsProvider = context.GetSettingsProvider();
        var fileSystem = context.GetFileSystem();

        var preferenceFilePath = settingsProvider.GetPreferenceFilePath();

        if (!string.IsNullOrEmpty(preferenceFilePath))
        {
            using var stream = fileSystem.File.Create(preferenceFilePath);
            source.Preferences?.FirstOrDefault()?.XmlConfig?.Save(stream);
        }
    }
}
