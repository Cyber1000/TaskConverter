using System.Text.Json.Serialization;
using System.Xml;
using TaskConverter.Plugin.GTD.ConversionHelper;

namespace TaskConverter.Plugin.GTD.Model;

public class Preferences
{
    [JsonPropertyName("com.dg.gtd.android.lite_preferences")]
    [JsonConverter(typeof(TaskInfoXmlConverter))]
    public XmlDocument? XmlConfig { get; set; }
}
