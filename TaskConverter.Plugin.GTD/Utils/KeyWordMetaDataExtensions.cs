using System.Text.Json;
using Ical.Net.DataTypes;
using TaskConverter.Plugin.Base.ConversionHelper;
using TaskConverter.Plugin.GTD.TodoModel;

namespace TaskConverter.Plugin.GTD.Utils;

public static class KeyWordMetaDataExtensions
{
    //TODO HH: tests
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
        return JsonSerializer.Serialize(obj);
    }

    public static KeyWordMetaData? KeyWordMetaDataFromString(this string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            var dto = JsonSerializer.Deserialize<IntermediateKeyWordMetaData>(json);
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
