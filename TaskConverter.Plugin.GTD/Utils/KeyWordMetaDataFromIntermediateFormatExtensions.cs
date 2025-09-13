using System.Drawing;
using Ical.Net;
using Ical.Net.CalendarComponents;
using NodaTime;
using TaskConverter.Commons.ConversionHelper;
using TaskConverter.Plugin.GTD.Conversion;
using TaskConverter.Plugin.GTD.TodoModel;

namespace TaskConverter.Plugin.GTD.Utils
{
    public static class KeyWordMetaDataFromIntermediateFormatExtensions
    {
        public static IEnumerable<KeyWordMetaData> GetKeyWordMetaDataList(this RecurringComponent recurringComponent, DateTimeZone timeZone)
        {
            if (recurringComponent.Categories is null)
                return Enumerable.Empty<KeyWordMetaData>();

            var properties = recurringComponent.Properties.ToDictionary(p => p.Name);
            return recurringComponent.Categories.Select(category => CreateOrGetMetaData(properties, category, timeZone));
        }

        private static KeyWordMetaData CreateOrGetMetaData(Dictionary<string, ICalendarProperty> properties, string category, DateTimeZone timeZone)
        {
            //TODO HH: Map to other types besides KeyWordType.Tag, based on first char (e.g. @, #)
            if (properties.TryGetValue(IntermediateFormatPropertyNames.CategoryMetaData(category), out var prop) && prop.Value is KeyWordMetaData existingMeta)
            {
                return existingMeta;
            }

            //TODO HH: add tests
            var now = DateTimeExtensions.GetCurrentDateTime(timeZone);

            return new KeyWordMetaData(category.ToIntWithHashFallback(), category, KeyWordType.Tag, now, now, Color.FromArgb(0));
        }
    }
}
