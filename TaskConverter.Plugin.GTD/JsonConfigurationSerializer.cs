using System.Text.Encodings.Web;
using System.Text.Json;
using NodaTime;
using TaskConverter.Commons.ConversionHelper;
using TaskConverter.Plugin.GTD.ConversionHelper;

namespace TaskConverter.Plugin.GTD
{
    public class JsonConfigurationSerializer : IJsonConfigurationSerializer
    {
        private readonly JsonSerializerOptions _options;

        public JsonConfigurationSerializer()
        {
            _options = InitSerializerOptions();
        }

        public virtual string Serialize<TValue>(TValue value)
        {
            return JsonSerializer.Serialize(value, _options);
        }

        public virtual TValue? Deserialize<TValue>(string json)
        {
            return JsonSerializer.Deserialize<TValue>(json, _options);
        }

        private static JsonSerializerOptions InitSerializerOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = new TaskInfoJsonNamingPolicy(),
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new ExactLocalDateTimeConverter<LocalDateTime?>(), new ExactLocalDateTimeConverter<LocalDateTime>(), new TaskInfoBoolConverter() },
            };

            return options;
        }
    }
}
