using System.Text.Json;
using Ical.Net.DataTypes;
using TaskConverter.Plugin.Base.ConversionHelper;
using TaskConverter.Plugin.GTD.TodoModel;

namespace TaskConverter.Plugin.GTD.Utils;

public static class KeyWordMetaDataExtensions
{
    public static string GetStringRepresentation(this KeyWordMetaData keyWordMetaData)
    {
        var obj = new
        {
            keyWordMetaData.Id,
            keyWordMetaData.Name,
            keyWordMetaData.KeyWordType,
            Created = keyWordMetaData.Created?.Value.ToString("o"),
            Modified = keyWordMetaData.Modified?.Value.ToString("o"),
            Color = keyWordMetaData.Color.ToStringRepresentation(),
            IsVisible = keyWordMetaData.IsVisible.ToStringRepresentation(),
        };
        string json = JsonSerializer.Serialize(obj);
        return IcalEscape(json);
    }

    public static KeyWordMetaData? KeyWordMetaDataFromString(this string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            string unescapedJson = IcalUnescape(json);
            var dto = JsonSerializer.Deserialize<IntermediateKeyWordMetaData>(unescapedJson);
            if (dto == null)
                return null;

            var name = dto.Name ?? string.Empty;

            return new KeyWordMetaData(
                dto.Id,
                name,
                $"{dto.KeyWordType}-{name}",
                dto.KeyWordType,
                ParseCalDateTime(dto.Created),
                ParseCalDateTime(dto.Modified),
                dto.Color.ColorFromStringRepresentation(),
                dto.IsVisible.ToBool()
            );
        }
        catch
        {
            return null;
        }
    }

    private static string IcalEscape(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        return input.Replace(@"\", @"\\").Replace(";", @"\;").Replace(",", @"\,").Replace("\r\n", @"\n").Replace("\n", @"\n");
    }

    private static string IcalUnescape(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        return input.Replace(@"\n", "\n").Replace(@"\,", ",").Replace(@"\;", ";").Replace(@"\\", @"\");
    }

    private static CalDateTime ParseCalDateTime(string? isoString)
    {
        if (string.IsNullOrWhiteSpace(isoString))
            return new CalDateTime(DateTime.MinValue);

        if (DateTime.TryParse(isoString, null, System.Globalization.DateTimeStyles.RoundtripKind, out var dt))
            return new CalDateTime(dt);

        return new CalDateTime(DateTime.MinValue);
    }

    private class IntermediateKeyWordMetaData
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public KeyWordType KeyWordType { get; set; }
        public string? Created { get; set; }
        public string? Modified { get; set; }
        public string? Color { get; set; }
        public string? IsVisible { get; set; }
    }
}
