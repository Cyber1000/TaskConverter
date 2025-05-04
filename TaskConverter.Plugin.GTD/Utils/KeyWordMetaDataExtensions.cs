using System.Drawing;
using Ical.Net;
using Ical.Net.CalendarComponents;
using NodaTime;
using TaskConverter.Plugin.GTD.Mapper;
using TaskConverter.Plugin.GTD.TodoModel;

namespace TaskConverter.Plugin.GTD.Utils
{
    public static class KeyWordMetaDataExtensions
    {
        public static IEnumerable<KeyWordMetaData> GetKeyWordMetaDataList(this RecurringComponent recurringComponent, DateTimeZone timeZone)
        {
            var propertiesOfTodo = recurringComponent.Properties.ToDictionary(p => p.Name);
            return propertiesOfTodo.GetKeyWordMetaData(recurringComponent.Categories, timeZone);
        }

        public static IEnumerable<KeyWordMetaData> GetKeyWordMetaData(this Dictionary<string, ICalendarProperty> propertiesOfTodo, IList<string> categories, DateTimeZone timeZone) =>
            categories?.Select(keyWordName => GetKeyWordMetaDataOrCreate(propertiesOfTodo, keyWordName, timeZone)).OfType<KeyWordMetaData>() ?? Enumerable.Empty<KeyWordMetaData>();

        private static KeyWordMetaData GetKeyWordMetaDataOrCreate(Dictionary<string, ICalendarProperty> propertiesOfTodo, string keyWordName, DateTimeZone timeZone)
        {
            //TODO HH: Maybe use StringToIntConverter instead of GetHashCode
            //TODO HH: map to other types as well additional to KeyWordType.Tag, by first char of string (e.g. @, #)
            return propertiesOfTodo.TryGetValue(KeyWordMappingBaseAction.CategoryMetaData(keyWordName), out var prop) && prop.Value is KeyWordMetaData metadata
                ? metadata
                : new KeyWordMetaData(keyWordName.GetHashCode(), keyWordName, KeyWordType.Tag, DateTimeExtensions.GetCurrentDateTime(timeZone), DateTimeExtensions.GetCurrentDateTime(timeZone), Color.FromArgb(0));
        }
    }
}
