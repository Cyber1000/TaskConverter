using AutoMapper;
using Ical.Net;
using Ical.Net.CalendarComponents;
using TaskConverter.Plugin.Base.Utils;
using TaskConverter.Plugin.GTD.TodoModel;

namespace TaskConverter.Plugin.GTD.Conversion;

public abstract class MapKeyWordsToIntermediateFormatBase<TSource, TDestination> : IMappingAction<TSource, TDestination>
    where TDestination : RecurringComponent
{
    public void Process(TSource source, TDestination destination, ResolutionContext context)
    {
        var keyWordList = context.GetKeyWordMetaDataGTDFormatDictionary();
        var keyWords = GetKeyWords(source);

        var keyWordMetaDataList = keyWords.Select(k => (k.keyWordType, k.Id)).GetExistingValues(keyWordList).ToList();

        foreach (var keyWordMetaData in keyWordMetaDataList)
        {
            destination.Categories.Add(keyWordMetaData.Name);
            destination.AddProperty(new CalendarProperty(IntermediateFormatPropertyNames.CategoryMetaData(keyWordMetaData.Name), keyWordMetaData));
        }
    }

    protected abstract IEnumerable<(int Id, KeyWordType keyWordType)> GetKeyWords(TSource source);
}
