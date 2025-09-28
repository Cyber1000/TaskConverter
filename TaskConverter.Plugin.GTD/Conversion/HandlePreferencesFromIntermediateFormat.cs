using System.Xml;
using AutoMapper;
using Ical.Net;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Utils;

namespace TaskConverter.Plugin.GTD.Conversion;

public class HandlePreferencesFromIntermediateFormat : IMappingAction<Calendar, GTDDataModel>
{
    public void Process(Calendar source, GTDDataModel destination, ResolutionContext context)
    {
        var settingsProvider = context.GetSettingsProvider();
        var fileSystem = context.GetFileSystem();

        var preferenceFilePath = settingsProvider.GetPreferenceFilePath();

        if (!string.IsNullOrEmpty(preferenceFilePath) && fileSystem.File.Exists(preferenceFilePath))
        {
            var content = fileSystem.File.ReadAllBytes(preferenceFilePath);
            using var memoryStream = new MemoryStream(content);
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(memoryStream);
            destination.Preferences = [new() { XmlConfig = xmlDocument }];
        }
    }
}
