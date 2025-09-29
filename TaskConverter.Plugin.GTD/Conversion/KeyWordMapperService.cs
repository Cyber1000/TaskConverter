using Ical.Net;
using Ical.Net.CalendarComponents;
using TaskConverter.Commons;
using TaskConverter.Commons.ConversionHelper;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.TodoModel;
using TaskConverter.Plugin.GTD.Utils;

namespace TaskConverter.Plugin.GTD.Conversion
{
    public class KeyWordMapperService : IKeyWordMapperService
    {
        public Dictionary<(KeyWordType keyWordType, int Id), KeyWordMetaData> GetKeyWordMetaDataGTDFormatDictionary(GTDDataModel gtdDataModel, ISettingsProvider settingsProvider)
        {
            var timeZone = settingsProvider.CurrentDateTimeZone;
            return (gtdDataModel.Context?.Select(c => ((KeyWordType.Context, c.Id), c as GTDExtendedModel)) ?? [])
                .Concat(gtdDataModel.Folder?.Select(f => ((KeyWordType.Folder, f.Id), f as GTDExtendedModel)) ?? [])
                .Concat(gtdDataModel.Tag?.Select(t => ((KeyWordType.Tag, t.Id), t as GTDExtendedModel)) ?? [])
                .Select(keyWordData => CreateKeyWordMetaData(keyWordData.Item1, keyWordData.Item2, settingsProvider))
                .ToDictionary(k => (k.KeyWordType, k.Id), k => k);
        }

        private static KeyWordMetaData CreateKeyWordMetaData((KeyWordType keyWordType, int Id) keyWord, GTDExtendedModel keyWordModel, ISettingsProvider settingsProvider)
        {
            var color = keyWordModel?.Color ?? -2;
            var timeZone = settingsProvider.CurrentDateTimeZone;

            return new KeyWordMetaData(
                keyWord.Id,
                MapCategory(keyWordModel?.Title ?? "", keyWord.keyWordType, settingsProvider),
                keyWord.keyWordType,
                keyWordModel?.Created.GetCalDateTime(timeZone) ?? DateTimeExtensions.GetCurrentDateTime(timeZone),
                keyWordModel?.Modified.GetCalDateTime(timeZone) ?? DateTimeExtensions.GetCurrentDateTime(timeZone),
                color.FromArgbWithFallback(),
                keyWordModel?.Visible ?? true
            );
        }

        public Dictionary<string, KeyWordMetaData> GetKeyWordMetaDataIntermediateFormatDictionary(Calendar calender, ISettingsProvider settingsProvider)
        {
            return calender
                .Todos.Select(t => GetKeyWordMetaDataList(t, settingsProvider))
                .Union(calender.Journals.Select(j => GetKeyWordMetaDataList(j, settingsProvider)))
                .SelectMany(t => t)
                .Distinct()
                .ToList()
                .ToDictionary(k => k.Name, k => k);
        }

        private static IEnumerable<KeyWordMetaData> GetKeyWordMetaDataList(RecurringComponent recurringComponent, ISettingsProvider settingsProvider)
        {
            if (recurringComponent.Categories is null)
                return Enumerable.Empty<KeyWordMetaData>();

            var properties = recurringComponent.Properties.ToDictionary(p => p.Name);
            return recurringComponent.Categories.Select(category => CreateOrGetMetaData(properties, category, settingsProvider));
        }

        private static KeyWordMetaData CreateOrGetMetaData(Dictionary<string, ICalendarProperty> properties, string category, ISettingsProvider settingsProvider)
        {
            if (properties.TryGetValue(IntermediateFormatPropertyNames.CategoryMetaData(category), out var prop) && prop.Value is KeyWordMetaData existingMeta)
            {
                return existingMeta;
            }

            var timeZone = settingsProvider.CurrentDateTimeZone;
            var now = DateTimeExtensions.GetCurrentDateTime(timeZone);

            var color = -1;
            var (newCategory, keyWordType) = GetCategoryAndKeyWordType(category, settingsProvider);
            return new KeyWordMetaData(newCategory.ToIntWithHashFallback(), newCategory, keyWordType, now, now, color.FromArgbWithFallback(), true);
        }

        private static (string, KeyWordType) GetCategoryAndKeyWordType(string category, ISettingsProvider settingsProvider)
        {
            var folderSymbol = settingsProvider.GetIntermediateFormatSymbol(KeyWordType.Folder);
            var contextSymbol = settingsProvider.GetIntermediateFormatSymbol(KeyWordType.Context);
            var folderGTDSymbol = settingsProvider.GetGTDFormatSymbol(KeyWordType.Folder);
            var contextGTDSymbol = settingsProvider.GetGTDFormatSymbol(KeyWordType.Context);
            if (folderSymbol?.Length > 0 && category.StartsWith(folderSymbol))
            {
                category = category[folderSymbol.Length..];
                if (folderGTDSymbol?.Length > 0)
                    category = $"{folderGTDSymbol}{category}";
                return (category, KeyWordType.Folder);
            }
            if (contextSymbol?.Length > 0 && category.StartsWith(contextSymbol))
            {
                category = category[contextSymbol.Length..];
                if (contextGTDSymbol?.Length > 0)
                    category = $"{contextGTDSymbol}{category}";
                return (category, KeyWordType.Context);
            }

            return (category, KeyWordType.Tag);
        }

        private static string MapCategory(string category, KeyWordType keyWordType, ISettingsProvider settingsProvider)
        {
            var folderSymbol = settingsProvider.GetIntermediateFormatSymbol(KeyWordType.Folder);
            var contextSymbol = settingsProvider.GetIntermediateFormatSymbol(KeyWordType.Context);
            if (keyWordType == KeyWordType.Folder && folderSymbol?.Length > 0 && !category.StartsWith(folderSymbol))
            {
                category = $"{folderSymbol}{category}";
            }
            if (keyWordType == KeyWordType.Context && contextSymbol?.Length > 0 && !category.StartsWith(contextSymbol))
            {
                category = $"{contextSymbol}{category}";
            }
            return category;
        }
    }
}
