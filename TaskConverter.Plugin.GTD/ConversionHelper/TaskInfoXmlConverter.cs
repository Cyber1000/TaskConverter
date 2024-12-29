using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

namespace TaskConverter.Plugin.GTD.ConversionHelper;

public class TaskInfoXmlConverter : JsonConverter<XmlDocument?>
{
    private static readonly XmlWriterSettings XmlWriterSettings = new()
    {
        Indent = true,
        IndentChars = "    ",
        NewLineChars = "\n",
        NewLineHandling = NewLineHandling.Replace,
        OmitXmlDeclaration = true
    };

    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(XmlDocument);

    public override bool HandleNull => true;

    public override XmlDocument? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var xmlString = reader.GetString();
        if (string.IsNullOrEmpty(xmlString))
            return null;

        var doc = new XmlDocument();
        doc.LoadXml(xmlString);
        return doc;
    }

    public override void Write(Utf8JsonWriter writer, XmlDocument? value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);

        if (value is null)
        {
            writer.WriteStringValue(string.Empty);
            return;
        }

        var stringBuilder = new StringBuilder();
        using var xmlWriter = XmlWriter.Create(stringBuilder, XmlWriterSettings);
        value.Save(xmlWriter);
        stringBuilder.Insert(0, "<?xml version='1.0' encoding='utf-8' standalone='yes' ?>\n");
        
        writer.WriteStringValue(stringBuilder.ToString());
    }
}
