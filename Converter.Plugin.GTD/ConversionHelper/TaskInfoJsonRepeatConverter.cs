using System.Text.Json;
using System.Text.Json.Serialization;
using Converter.Plugin.GTD.Model;

namespace Converter.Plugin.GTD.ConversionHelper;

public class TaskInfoJsonRepeatConverter : JsonConverter<RepeatInfo?>
{
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(RepeatInfo?);

    public override bool HandleNull => true;

    public TaskInfoJsonRepeatConverter() { }

    public override RepeatInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
            return new RepeatInfo(stringValue);
        }
    }

    public override void Write(Utf8JsonWriter writer, RepeatInfo? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value == null ? "" : value.ToString());
    }
}
