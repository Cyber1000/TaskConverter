using AutoMapper;
using NodaTime;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.TodoModel;

namespace TaskConverter.Plugin.GTD.Conversion;

public static class GTDDataModelExtensions
{
    private const string keyWordListName = "KeyWordList";
    private const string timeZoneName = "TimeZone";

    public static void InitializeResolutionContextForCalendarMapping(this IMappingOperationOptions options, GTDDataModel source, DateTimeZone timeZone)
    {
        var keyWordList = (source.Context?.Select(c => ((KeyWordType.Context, c.Id), c as GTDExtendedModel)) ?? [])
            .Concat(source.Folder?.Select(f => ((KeyWordType.Folder, f.Id), f as GTDExtendedModel)) ?? [])
            .Concat(source.Tag?.Select(t => ((KeyWordType.Tag, t.Id), t as GTDExtendedModel)) ?? [])
            .ToDictionary();

        options.Items[keyWordListName] = keyWordList;
        options.Items[timeZoneName] = timeZone;
    }

    public static void InitializeResolutionContextForGDTMapping(this IMappingOperationOptions options, DateTimeZone timeZone)
    {
        options.Items[timeZoneName] = timeZone;
    }

    public static Dictionary<(KeyWordType, int), GTDExtendedModel>? GetKeyWordList(this ResolutionContext context)
    {
        return context.Items[keyWordListName] as Dictionary<(KeyWordType, int), GTDExtendedModel>;
    }

    public static DateTimeZone GetTimeZone(this ResolutionContext context)
    {
        return (context.Items[timeZoneName] as DateTimeZone)!;
    }
}
