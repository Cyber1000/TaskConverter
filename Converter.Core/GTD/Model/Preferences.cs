using System.Text.Json.Serialization;
using System.Xml;
using Converter.Core.GTD.ConversionHelper;

namespace Converter.Core.GTD.Model
{
    public class Preferences
    {
        [JsonPropertyName("com.dg.gtd.android.lite_preferences")]
        [JsonConverter(typeof(TaskInfoXmlConverter))]
        public XmlDocument? XmlConfig { get; set; }
    }
}
