using Ical.Net;
using Ical.Net.CalendarComponents;
using NodaTime;
using TaskConverter.Commons.ConversionHelper;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.TodoModel;
using TaskConverter.Plugin.GTD.Utils;

namespace TaskConverter.Plugin.GTD.Conversion
{
    public class KeyWordMapperService : IKeyWordMapperService
    {
        public Dictionary<(KeyWordType keyWordType, int Id), KeyWordMetaData> GetKeyWordMetaDataGTDFormatDictionary(GTDDataModel gtdDataModel, DateTimeZone timeZone)
        {
            return (gtdDataModel.Context?.Select(c => ((KeyWordType.Context, c.Id), c as GTDExtendedModel)) ?? [])
                .Concat(gtdDataModel.Folder?.Select(f => ((KeyWordType.Folder, f.Id), f as GTDExtendedModel)) ?? [])
                .Concat(gtdDataModel.Tag?.Select(t => ((KeyWordType.Tag, t.Id), t as GTDExtendedModel)) ?? [])
                .Select(keyWordData => CreateKeyWordMetaData(keyWordData.Item1, keyWordData.Item2, timeZone))
                .ToDictionary(k => (k.KeyWordType, k.Id), k => k);
        }

        private static KeyWordMetaData CreateKeyWordMetaData((KeyWordType keyWordType, int Id) keyWord, GTDExtendedModel keyWordModel, DateTimeZone timeZone)
        {
            var color = keyWordModel?.Color ?? -2;

            return new KeyWordMetaData(
                keyWord.Id,
                keyWordModel?.Title ?? "",
                keyWord.keyWordType,
                keyWordModel?.Created.GetIDateTime(timeZone) ?? DateTimeExtensions.GetCurrentDateTime(timeZone),
                keyWordModel?.Modified.GetIDateTime(timeZone) ?? DateTimeExtensions.GetCurrentDateTime(timeZone),
                color.FromArgbWithFallback(),
                keyWordModel?.Visible ?? true
            );
        }

        public Dictionary<string, KeyWordMetaData> GetKeyWordMetaDataIntermediateFormatDictionary(Calendar calender, DateTimeZone timeZone)
        {
            return calender
                .Todos.Select(t => GetKeyWordMetaDataList(t, timeZone))
                .Union(calender.Journals.Select(j => GetKeyWordMetaDataList(j, timeZone)))
                .SelectMany(t => t)
                .Distinct()
                .ToList()
                .ToDictionary(k => k.Name, k => k);
        }

        private static IEnumerable<KeyWordMetaData> GetKeyWordMetaDataList(RecurringComponent recurringComponent, DateTimeZone timeZone)
        {
            if (recurringComponent.Categories is null)
                return Enumerable.Empty<KeyWordMetaData>();

            var properties = recurringComponent.Properties.ToDictionary(p => p.Name);
            return recurringComponent.Categories.Select(category => CreateOrGetMetaData(properties, category, timeZone));
        }

        private static KeyWordMetaData CreateOrGetMetaData(Dictionary<string, ICalendarProperty> properties, string category, DateTimeZone timeZone)
        {
            //TODO HH: Map to other types besides KeyWordType.Tag, based on first char (e.g. @, #) and add color to fallback
            if (properties.TryGetValue(IntermediateFormatPropertyNames.CategoryMetaData(category), out var prop) && prop.Value is KeyWordMetaData existingMeta)
            {
                return existingMeta;
            }

            //TODO HH: add tests for fallback
            var now = DateTimeExtensions.GetCurrentDateTime(timeZone);

            var color = -1;
            return new KeyWordMetaData(category.ToIntWithHashFallback(), category, KeyWordType.Tag, now, now, color.FromArgbWithFallback(), true);
        }
    }
}
