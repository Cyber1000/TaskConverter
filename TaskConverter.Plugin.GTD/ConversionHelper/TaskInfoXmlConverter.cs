using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

namespace TaskConverter.Plugin.GTD.ConversionHelper;

public class TaskInfoXmlConverter : JsonConverter<XmlDocument?>
{
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(XmlDocument);

    public override bool HandleNull => true;

    public override XmlDocument? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var xmlString = reader.GetString();
        if (string.IsNullOrEmpty(xmlString))
            return null;

        XmlDocument doc = new();
        doc.LoadXml(xmlString);
        return doc;
    }

    public override void Write(Utf8JsonWriter writer, XmlDocument? value, JsonSerializerOptions options)
    {
        var output = "";
        if (value is not null)
        {
            var stringBuilder = new StringBuilder();
            XmlWriterSettings xmlWriterSettings =
                new()
                {
                    Indent = true,
                    IndentChars = "    ",
                    NewLineChars = "\n",
                    NewLineHandling = NewLineHandling.Replace,
                    OmitXmlDeclaration = true
                };
            using var xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSettings);
            value.Save(xmlWriter);
            stringBuilder.Insert(0, "<?xml version='1.0' encoding='utf-8' standalone='yes' ?>\n");
            output = stringBuilder.ToString();
        }

        writer.WriteStringValue(output);
    }
}
