using System.Text.Json;
using System.Text.Json.Serialization;

namespace Converter.Core.GTD.ConversionHelper;

public class TaskInfoBoolConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var boolean = reader.GetSingle();
        return boolean != 0;
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value ? 1 : 0);
    }
}
