using System.Drawing;
using AutoMapper;
using Ical.Net;
using Ical.Net.CalendarComponents;
using NodaTime;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.TodoModel;
using TaskConverter.Plugin.GTD.Utils;

namespace TaskConverter.Plugin.GTD.Conversion;

public abstract class KeyWordMappingBaseAction
{
    //TODO HH: move to other class?
    public static string CategoryMetaData(string keyWordName) => $"X-Category-{keyWordName}";
}

public abstract class KeyWordMappingBaseAction<TSource, TDestination> : KeyWordMappingBaseAction, IMappingAction<TSource, TDestination>
    where TDestination : RecurringComponent
{
    public void Process(TSource source, TDestination destination, ResolutionContext context)
    {
        var keyWordList = context.GetKeyWordList();
        var keyWords = GetKeyWords(source);
        var timeZone = context.GetTimeZone();

        var keyWordMetaDataList = keyWords.Select(keyWord => CreateKeyWordMetaData(keyWordList, keyWord, timeZone)).Cast<KeyWordMetaData>().ToList();

        foreach (var keyWordMetaData in keyWordMetaDataList)
        {
            destination.Categories.Add(keyWordMetaData.Name);
            //TODO HH: fix spaces
            //TODO HH: serialize custom properties in ICal (Color and others too)
            destination.AddProperty(new CalendarProperty(CategoryMetaData(keyWordMetaData.Name), keyWordMetaData));
        }
    }

    protected abstract IEnumerable<(int Id, KeyWordType keyWordType)> GetKeyWords(TSource source);

    private static KeyWordMetaData CreateKeyWordMetaData(Dictionary<(KeyWordType, int), GTDExtendedModel>? keyWordList, (int Id, KeyWordType keyWordType) keyWord, DateTimeZone timeZone)
    {
        GTDExtendedModel? keyWordModel = null;
        keyWordList?.TryGetValue((keyWord.keyWordType, keyWord.Id), out keyWordModel);
        return new KeyWordMetaData(
            keyWord.Id,
            keyWordModel?.Title ?? "",
            keyWord.keyWordType,
            keyWordModel?.Created.GetIDateTime(timeZone) ?? DateTimeExtensions.GetCurrentDateTime(timeZone),
            keyWordModel?.Modified.GetIDateTime(timeZone) ?? DateTimeExtensions.GetCurrentDateTime(timeZone),
            Color.FromArgb(keyWordModel?.Color ?? 0)
        );
    }
}
