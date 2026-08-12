using System.Globalization;
using Ical.Net;
using Ical.Net.DataTypes;

namespace TaskConverter.Plugin.GTD.Utils;

public static class CalendarPropertyExtensions
{
    private static readonly string[] icalDateTimePatterns = ["yyyyMMdd'T'HHmmss'Z'", "yyyyMMdd'T'HHmmss", "yyyyMMdd"];

    /// <summary>
    /// Reads a date from a custom property. The value is a <see cref="CalDateTime"/> while the
    /// calendar is still an object graph, but a plain string once it has been parsed from text -
    /// ical.net cannot know that an X- property holds a date. Asking only for the typed value made
    /// every such date vanish on the way back.
    /// </summary>
    public static CalDateTime? GetCalDateTime(this CalendarPropertyList properties, string propertyName)
    {
        return properties.Get<object>(propertyName) switch
        {
            CalDateTime calDateTime => calDateTime,
            string text => ParseIcalDateTime(text),
            _ => null,
        };
    }

    /// <summary>
    /// Returns null when the property is absent, so the caller can tell "not carried" from "false".
    /// </summary>
    public static bool? GetBool(this CalendarPropertyList properties, string propertyName)
    {
        return bool.TryParse(properties.Get<string>(propertyName), out var value) ? value : null;
    }

    private static CalDateTime? ParseIcalDateTime(string text)
    {
        var isUtc = text.EndsWith('Z');
        if (!DateTime.TryParseExact(text, icalDateTimePatterns, CultureInfo.InvariantCulture, isUtc ? DateTimeStyles.AdjustToUniversal : DateTimeStyles.None, out var parsed))
            return null;

        return isUtc ? new CalDateTime(parsed, "UTC") : new CalDateTime(parsed);
    }
}
