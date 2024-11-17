using System.Text.Json;
using System.Text.Json.Serialization;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.ConversionHelper;

public class TaskInfoJsonRepeatConverter : JsonConverter<GTDRepeatInfoModel?>
{
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(GTDRepeatInfoModel?);

    public override bool HandleNull => true;

    public TaskInfoJsonRepeatConverter() { }

    public override GTDRepeatInfoModel? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString()?.ToLower();
        if (stringValue == null)
            throw new Exception($"Can't Convert {stringValue} to enum {typeToConvert.Name}");

        if (stringValue == string.Empty || stringValue == "norepeat")
        {
            return null;
        }
        else
        {
            return new GTDRepeatInfoModel(stringValue);
        }
    }

    public override void Write(Utf8JsonWriter writer, GTDRepeatInfoModel? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value == null ? "" : value.ToString());
    }
}
