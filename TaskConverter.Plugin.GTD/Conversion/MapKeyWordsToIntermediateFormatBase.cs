using System.Drawing;
using AutoMapper;
using Ical.Net;
using Ical.Net.CalendarComponents;
using NodaTime;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.TodoModel;
using TaskConverter.Plugin.GTD.Utils;

namespace TaskConverter.Plugin.GTD.Conversion;

public abstract class MapKeyWordsToIntermediateFormatBase<TSource, TDestination> : IMappingAction<TSource, TDestination>
    where TDestination : RecurringComponent
{
    public void Process(TSource source, TDestination destination, ResolutionContext context)
    {
        var keyWordList = context.GetKeyWordGTDFormatDictionary();
        var keyWords = GetKeyWords(source);
        var timeZone = context.GetTimeZone();

        var keyWordMetaDataList = keyWords.Select(keyWord => CreateKeyWordMetaData(keyWordList, keyWord, timeZone)).Cast<KeyWordMetaData>().ToList();

        foreach (var keyWordMetaData in keyWordMetaDataList)
        {
            //TODO HH: handle same name of different Types?
            destination.Categories.Add(keyWordMetaData.Name);
            destination.AddProperty(new CalendarProperty(IntermediateFormatPropertyNames.CategoryMetaData(keyWordMetaData.Name), keyWordMetaData));
        }
    }

    protected abstract IEnumerable<(int Id, KeyWordType keyWordType)> GetKeyWords(TSource source);

    private static KeyWordMetaData CreateKeyWordMetaData(Dictionary<(KeyWordType, int), GTDExtendedModel>? keyWordList, (int Id, KeyWordType keyWordType) keyWord, DateTimeZone timeZone)
    {
        GTDExtendedModel? keyWordModel = null;
        keyWordList?.TryGetValue((keyWord.keyWordType, keyWord.Id), out keyWordModel);
        //TODO HH: exception if not found?
        return new KeyWordMetaData(
            keyWord.Id,
            keyWordModel?.Title ?? "",
            keyWord.keyWordType,
            keyWordModel?.Created.GetIDateTime(timeZone) ?? DateTimeExtensions.GetCurrentDateTime(timeZone),
            keyWordModel?.Modified.GetIDateTime(timeZone) ?? DateTimeExtensions.GetCurrentDateTime(timeZone),
            //TODO HH: Add Color only for Folder, Context - Visible for all Keywords
            Color.FromArgb(keyWordModel?.Color ?? 0)
        );
    }
}
