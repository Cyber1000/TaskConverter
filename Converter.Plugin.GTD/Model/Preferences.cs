using System.Text.Json.Serialization;
using System.Xml;
using Converter.Plugin.GTD.ConversionHelper;

namespace Converter.Plugin.GTD.Model;

public class Preferences
{
    [JsonPropertyName("com.dg.gtd.android.lite_preferences")]
    [JsonConverter(typeof(TaskInfoXmlConverter))]
    public XmlDocument? XmlConfig { get; set; }
}
