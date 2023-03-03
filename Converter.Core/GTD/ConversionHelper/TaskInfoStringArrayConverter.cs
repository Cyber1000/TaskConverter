using System.Text.Json;
using System.Text.Json.Serialization;

namespace Converter.Core.GTD.ConversionHelper;

public class TaskInfoStringArrayConverter : JsonConverter<string[]>
{
    public override string[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();
        return stringValue?.Split("\n");
    }

    public override void Write(Utf8JsonWriter writer, string[]? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value is null ? "" : string.Join("\n", value));
    }
}
