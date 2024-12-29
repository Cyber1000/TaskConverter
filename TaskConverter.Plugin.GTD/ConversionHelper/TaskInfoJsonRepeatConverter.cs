using System.Text.Json;
using System.Text.Json.Serialization;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.ConversionHelper;

public class TaskInfoJsonRepeatConverter : JsonConverter<GTDRepeatInfoModel?>
{
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(GTDRepeatInfoModel?);

    public override bool HandleNull => true;

    public override GTDRepeatInfoModel? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);

        var stringValue = reader.GetString()?.ToLowerInvariant();
        if (string.IsNullOrEmpty(stringValue) || stringValue == "norepeat")
        {
            return null;
        }

        return new GTDRepeatInfoModel(stringValue);
    }

    public override void Write(Utf8JsonWriter writer, GTDRepeatInfoModel? value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        writer.WriteStringValue(value?.ToString() ?? string.Empty);
    }
}
