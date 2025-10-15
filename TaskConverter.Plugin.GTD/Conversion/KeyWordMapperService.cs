using Ical.Net;
using Ical.Net.CalendarComponents;
using NodaTime;
using TaskConverter.Plugin.Base;
using TaskConverter.Plugin.Base.ConversionHelper;
using TaskConverter.Plugin.Base.Utils;
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
            return (gtdDataModel.Context?.Select(c => (Id: (KeyWordType.Context, c.Id), Data: (c.Title, c.Color, (LocalDateTime?)c.Created, (LocalDateTime?)c.Modified, c.Visible))) ?? [])
                .Concat(gtdDataModel.Folder?.Select(f => (Id: (KeyWordType.Folder, f.Id), Data: (f.Title, f.Color, (LocalDateTime?)f.Created, (LocalDateTime?)f.Modified, f.Visible))) ?? [])
                .Concat(gtdDataModel.Tag?.Select(t => (Id: (KeyWordType.Tag, t.Id), Data: (t.Title, t.Color, (LocalDateTime?)t.Created, (LocalDateTime?)t.Modified, t.Visible))) ?? [])
                .Concat(GetStatusDataFromGTD().Select(t => (Id: (KeyWordType.Status, t.Id), Data: ((string?)t.Title, 5, (LocalDateTime?)null, (LocalDateTime?)null, true))) ?? [])
                .Select(keyWordData => CreateKeyWordMetaDataFromGTD(keyWordData.Id, keyWordData.Data, settingsProvider))
                .ToDictionary(k => (k.KeyWordType, k.Id), k => k);
        }

        private static IEnumerable<(int Id, string Title)> GetStatusDataFromGTD()
        {
            var excludeStatuses = new List<Status> { Status.None, Status.Canceled };
            return Enum.GetValues<Status>().Where(s => !excludeStatuses.Contains(s)).Select(s => (Id: s.ToString().ToIntWithHashFallback(), Title: s.ToString()));
        }

        private static KeyWordMetaData CreateKeyWordMetaDataFromGTD(
            (KeyWordType keyWordType, int Id) keyWord,
            (string? Title, int? Color, LocalDateTime? Created, LocalDateTime? Modified, bool? Visible) data,
            ISettingsProvider settingsProvider
        )
        {
            var color = data.Color ?? -2;
            var timeZone = settingsProvider.CurrentDateTimeZone;

            return new KeyWordMetaData(
                keyWord.Id,
                MapCategoryFromGTD(data.Title ?? "", keyWord.keyWordType, settingsProvider),
                MapCategoryFromGTD(data.Title ?? "", keyWord.keyWordType, settingsProvider, false),
                keyWord.keyWordType,
                data.Created.GetCalDateTime(timeZone) ?? DateTimeExtensions.GetCurrentDateTime(timeZone),
                data.Modified.GetCalDateTime(timeZone) ?? DateTimeExtensions.GetCurrentDateTime(timeZone),
                color.FromArgbWithFallback(),
                data.Visible ?? true
            );
        }

        private static string MapCategoryFromGTD(string category, KeyWordType keyWordType, ISettingsProvider settingsProvider, bool useSymbol = true)
        {
            var folderGTDSymbol = settingsProvider.GetGTDFormatSymbol(KeyWordType.Folder);
            var contextGTDSymbol = settingsProvider.GetGTDFormatSymbol(KeyWordType.Context);
            var statusGTDSymbol = settingsProvider.GetGTDFormatSymbol(KeyWordType.Status);
            var folderSymbol = useSymbol ? settingsProvider.GetIntermediateFormatSymbol(KeyWordType.Folder) : $"{KeyWordType.Folder}-";
            var contextSymbol = useSymbol ? settingsProvider.GetIntermediateFormatSymbol(KeyWordType.Context) : $"{KeyWordType.Context}-";
            var statusSymbol = useSymbol ? settingsProvider.GetIntermediateFormatSymbol(KeyWordType.Status) : $"{KeyWordType.Status}-";

            return keyWordType switch
            {
                KeyWordType.Context => category.RemovePrefix(contextGTDSymbol).AddPrefix(contextSymbol),
                KeyWordType.Folder => category.RemovePrefix(folderGTDSymbol).AddPrefix(folderSymbol),
                KeyWordType.Status => category.RemovePrefix(statusGTDSymbol).AddPrefix(statusSymbol),
                _ => category,
            };
        }

        public Dictionary<string, KeyWordMetaData> GetKeyWordMetaDataIntermediateFormatDictionary(Calendar calender, ISettingsProvider settingsProvider)
        {
            return calender
                .Todos.Select(t => GetKeyWordMetaDataListFromIntermediateFormat(t, settingsProvider))
                .Union(calender.Journals.Select(j => GetKeyWordMetaDataListFromIntermediateFormat(j, settingsProvider)))
                .SelectMany(t => t)
                .Distinct()
                .ToList()
                .ToDictionary(k => k.Name, k => k);
        }

        private static IEnumerable<KeyWordMetaData> GetKeyWordMetaDataListFromIntermediateFormat(RecurringComponent recurringComponent, ISettingsProvider settingsProvider)
        {
            if (recurringComponent.Categories is null)
                return Enumerable.Empty<KeyWordMetaData>();

            var properties = recurringComponent.Properties.ToDictionary(p => p.Name);
            return recurringComponent.Categories.Select(category => CreateOrGetMetaDataFromIntermediateFormat(properties, category, settingsProvider));
        }

        private static KeyWordMetaData CreateOrGetMetaDataFromIntermediateFormat(Dictionary<string, ICalendarProperty> properties, string category, ISettingsProvider settingsProvider)
        {
            var (categoryNameWithType, _) = GetCategoryAndKeyWordTypeFromIntermediateFormat(category, settingsProvider, false);
            if (properties.TryGetValue(IntermediateFormatPropertyNames.CategoryMetaData(categoryNameWithType), out var prop) && prop.Value is KeyWordMetaData existingMeta)
            {
                return existingMeta;
            }

            var timeZone = settingsProvider.CurrentDateTimeZone;
            var now = DateTimeExtensions.GetCurrentDateTime(timeZone);

            var color = -1;
            var (newCategory, keyWordType) = GetCategoryAndKeyWordTypeFromIntermediateFormat(category, settingsProvider);
            return new KeyWordMetaData(newCategory.ToIntWithHashFallback(), newCategory, $"{keyWordType}-{newCategory}", keyWordType, now, now, color.FromArgbWithFallback(), true);
        }

        private static (string, KeyWordType) GetCategoryAndKeyWordTypeFromIntermediateFormat(string category, ISettingsProvider settingsProvider, bool useSymbol = true)
        {
            var folderSymbol = settingsProvider.GetIntermediateFormatSymbol(KeyWordType.Folder);
            var contextSymbol = settingsProvider.GetIntermediateFormatSymbol(KeyWordType.Context);
            var statusSymbol = settingsProvider.GetIntermediateFormatSymbol(KeyWordType.Status);
            var folderGTDSymbol = useSymbol ? settingsProvider.GetGTDFormatSymbol(KeyWordType.Folder) : $"{KeyWordType.Folder}-";
            var contextGTDSymbol = useSymbol ? settingsProvider.GetGTDFormatSymbol(KeyWordType.Context) : $"{KeyWordType.Context}-";
            var statusGTDSymbol = useSymbol ? settingsProvider.GetGTDFormatSymbol(KeyWordType.Status) : $"{KeyWordType.Status}-";

            if (category.StartsWithPrefix(folderSymbol))
            {
                category = category.RemovePrefix(folderSymbol).AddPrefix(folderGTDSymbol);
                return (category, KeyWordType.Folder);
            }
            if (category.StartsWithPrefix(contextSymbol))
            {
                category = category.RemovePrefix(contextSymbol).AddPrefix(contextGTDSymbol);
                return (category, KeyWordType.Context);
            }
            if (category.StartsWithPrefix(statusSymbol))
            {
                category = category.RemovePrefix(statusSymbol).AddPrefix(statusGTDSymbol);
                return (category, KeyWordType.Status);
            }

            return (category, KeyWordType.Tag);
        }
    }
}
