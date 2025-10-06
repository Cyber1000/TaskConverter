using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskConverter.Plugin.Base.ConversionHelper.Json;

public class TaskInfoEnumConverter : JsonConverter<Enum>
{
    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsEnum || IsNullableEnum(typeToConvert);

    private static bool IsNullableEnum(Type t)
    {
        Type? u = Nullable.GetUnderlyingType(t);
        return u != null && u.IsEnum;
    }

    public override Enum? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();
        if (stringValue == null || !int.TryParse(stringValue, out var intValue))
            throw new Exception($"Can't Convert {stringValue} to enum");
        return (Enum?)Enum.ToObject(typeToConvert, intValue);
    }

    public override void Write(Utf8JsonWriter writer, Enum value, JsonSerializerOptions options)
    {
        var intValue = Convert.ToInt32(value);
        writer.WriteStringValue(intValue.ToString());
    }
}
