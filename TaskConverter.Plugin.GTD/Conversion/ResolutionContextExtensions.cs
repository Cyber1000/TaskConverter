using AutoMapper;
using NodaTime;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.TodoModel;

namespace TaskConverter.Plugin.GTD.Conversion;

public static class ResolutionContextExtensions
{
    private const string keyWordGTDFormatDictionaryName = "KeyWordGTDFormatDictionary";
    private const string keyWordMetaDataIntermediateFormatDictionaryName = "KeyWordMetaDataIntermediateFormatDictionary";
    private const string timeZoneName = "TimeZone";

    public static void InitializeResolutionContextForMappingToIntermediateFormat(this IMappingOperationOptions options, GTDDataModel source, DateTimeZone timeZone)
    {
        var keyWordMapperService = new KeyWordMapperService();

        options.Items[keyWordGTDFormatDictionaryName] = keyWordMapperService.CreateKeyWordMetaDataList(source, timeZone);
        options.Items[timeZoneName] = timeZone;
    }

    public static void InitializeResolutionContextForMappingFromIntermediateFormat(this IMappingOperationOptions options, Ical.Net.Calendar source, DateTimeZone timeZone)
    {
        var keyWordMapperService = new KeyWordMapperService();

        options.Items[keyWordMetaDataIntermediateFormatDictionaryName] = keyWordMapperService.GetKeyWordMetaDataIntermediateFormatDictionary(source, timeZone);
        options.Items[timeZoneName] = timeZone;
    }

    public static Dictionary<(KeyWordType keyWordType, int Id), KeyWordMetaData>? GetKeyWordMetaDataGTDFormatDictionary(this ResolutionContext context)
    {
        return context.Items[keyWordGTDFormatDictionaryName] as Dictionary<(KeyWordType keyWordType, int Id), KeyWordMetaData>;
    }

    public static Dictionary<string, KeyWordMetaData>? GetKeyWordMetaDataIntermediateFormatDictionary(this ResolutionContext context)
    {
        return context.Items[keyWordMetaDataIntermediateFormatDictionaryName] as Dictionary<string, KeyWordMetaData>;
    }

    public static DateTimeZone GetTimeZone(this ResolutionContext context)
    {
        return (context.Items[timeZoneName] as DateTimeZone)!;
    }
}
