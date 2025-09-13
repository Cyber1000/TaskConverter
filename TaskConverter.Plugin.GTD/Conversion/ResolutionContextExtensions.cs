using AutoMapper;
using NodaTime;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.TodoModel;
using TaskConverter.Plugin.GTD.Utils;

namespace TaskConverter.Plugin.GTD.Conversion;

public static class ResolutionContextExtensions
{
    private const string keyWordGTDFormatDictionaryName = "KeyWordGTDFormatDictionary";
    private const string keyWordMetaDataIntermediateFormatDictionaryName = "KeyWordMetaDataIntermediateFormatDictionary";
    private const string timeZoneName = "TimeZone";

    public static void InitializeResolutionContextForMappingToIntermediateFormat(this IMappingOperationOptions options, GTDDataModel source, DateTimeZone timeZone)
    {
        var keyWordGTDFormatDictionary = (source.Context?.Select(c => ((KeyWordType.Context, c.Id), c as GTDExtendedModel)) ?? [])
            .Concat(source.Folder?.Select(f => ((KeyWordType.Folder, f.Id), f as GTDExtendedModel)) ?? [])
            .Concat(source.Tag?.Select(t => ((KeyWordType.Tag, t.Id), t as GTDExtendedModel)) ?? [])
            .ToDictionary();

        options.Items[keyWordGTDFormatDictionaryName] = keyWordGTDFormatDictionary;
        options.Items[timeZoneName] = timeZone;
    }

    public static void InitializeResolutionContextForMappingFromIntermediateFormat(this IMappingOperationOptions options, Ical.Net.Calendar source, DateTimeZone timeZone)
    {
        var keyWordMetaDataIntermediateFormatDictionary = source
            .Todos.Select(t => t.GetKeyWordMetaDataList(timeZone))
            .Union(source.Journals.Select(j => j.GetKeyWordMetaDataList(timeZone)))
            .SelectMany(t => t)
            .Distinct()
            .ToList()
            .ToDictionary(k => k.Name, k => k);
        
        options.Items[keyWordMetaDataIntermediateFormatDictionaryName] = keyWordMetaDataIntermediateFormatDictionary;       
        options.Items[timeZoneName] = timeZone;
    }

    public static Dictionary<(KeyWordType, int), GTDExtendedModel>? GetKeyWordGTDFormatDictionary(this ResolutionContext context)
    {
        return context.Items[keyWordGTDFormatDictionaryName] as Dictionary<(KeyWordType, int), GTDExtendedModel>;
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
