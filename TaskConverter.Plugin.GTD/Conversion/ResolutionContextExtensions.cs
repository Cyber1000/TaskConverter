using System.IO.Abstractions;
using AutoMapper;
using TaskConverter.Commons;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.TodoModel;

namespace TaskConverter.Plugin.GTD.Conversion;

public static class ResolutionContextExtensions
{
    private const string keyWordGTDFormatDictionaryName = "KeyWordGTDFormatDictionary";
    private const string keyWordMetaDataIntermediateFormatDictionaryName = "KeyWordMetaDataIntermediateFormatDictionary";
    private const string settingsProviderName = "SettingsProvider";
    private const string fileSystemName = "FileSystem";

    public static void InitializeResolutionContextForMappingToIntermediateFormat(
        this IMappingOperationOptions options,
        GTDDataModel source,
        ISettingsProvider settingsProvider,
        IFileSystem fileSystem
    )
    {
        var keyWordMapperService = new KeyWordMapperService();

        options.Items[keyWordGTDFormatDictionaryName] = keyWordMapperService.GetKeyWordMetaDataGTDFormatDictionary(source, settingsProvider.CurrentDateTimeZone);
        options.Items[settingsProviderName] = settingsProvider;
        options.Items[fileSystemName] = fileSystem;
    }

    public static void InitializeResolutionContextForMappingFromIntermediateFormat(
        this IMappingOperationOptions options,
        Ical.Net.Calendar source,
        ISettingsProvider settingsProvider,
        IFileSystem fileSystem
    )
    {
        var keyWordMapperService = new KeyWordMapperService();

        options.Items[keyWordMetaDataIntermediateFormatDictionaryName] = keyWordMapperService.GetKeyWordMetaDataIntermediateFormatDictionary(source, settingsProvider.CurrentDateTimeZone);
        options.Items[settingsProviderName] = settingsProvider;
        options.Items[fileSystemName] = fileSystem;
    }

    public static Dictionary<(KeyWordType keyWordType, int Id), KeyWordMetaData>? GetKeyWordMetaDataGTDFormatDictionary(this ResolutionContext context)
    {
        return context.Items[keyWordGTDFormatDictionaryName] as Dictionary<(KeyWordType keyWordType, int Id), KeyWordMetaData>;
    }

    public static Dictionary<string, KeyWordMetaData>? GetKeyWordMetaDataIntermediateFormatDictionary(this ResolutionContext context)
    {
        return context.Items[keyWordMetaDataIntermediateFormatDictionaryName] as Dictionary<string, KeyWordMetaData>;
    }

    public static ISettingsProvider GetSettingsProvider(this ResolutionContext context)
    {
        return (context.Items[settingsProviderName] as ISettingsProvider)!;
    }

    public static IFileSystem GetFileSystem(this ResolutionContext context)
    {
        return (context.Items[fileSystemName] as IFileSystem)!;
    }
}
