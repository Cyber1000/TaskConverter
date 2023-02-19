using System.Text.Json.Serialization;

namespace Converter.Core.GTD.ConversionHelper
{
    public class JsonDateTimeConverterAttribute : JsonConverterAttribute
    {
        public readonly string ConversionFormat;

        public JsonDateTimeConverterAttribute(string conversionFormat = "yyyy-MM-dd HH:mm:ss.fff")
        {
            ConversionFormat = conversionFormat;
        }

        public override JsonConverter? CreateConverter(Type typeToConvert)
        {
            return typeToConvert switch
            {
                Type dateTime when dateTime == typeof(DateTime) => new TaskInfoDateTimeConverter(ConversionFormat),
                Type dateTimeNullable when dateTimeNullable == typeof(DateTime?) => new TaskInfoDateTimeNullableConverter(ConversionFormat),
                _ => throw new Exception("Can only use this attribute on DateTime properties"),
            };
        }
    }
}
