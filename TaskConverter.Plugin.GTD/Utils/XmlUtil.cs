using System.Xml;
using Microsoft.XmlDiffPatch;

namespace TaskConverter.Plugin.GTD.Utils;

public static class XmlUtil
{
    public static (bool hasError, string xmlDiff) DiffXml(XmlDocument originalXml, XmlDocument recreatedXml)
    {
        ArgumentNullException.ThrowIfNull(originalXml);
        ArgumentNullException.ThrowIfNull(recreatedXml);

        using var stream = new MemoryStream();
        using var xmlWriter = XmlWriter.Create(stream);
        var xmldiff = new XmlDiff();

        var hasError = !xmldiff.Compare(originalXml, recreatedXml, xmlWriter);
        xmlWriter.Flush();
        stream.Position = 0;

        using var reader = new StreamReader(stream);
        var xmlDiff = reader.ReadToEnd();
        return (hasError, xmlDiff);
    }

    public static string MaskForJson(string xmlString)
    {
        ArgumentNullException.ThrowIfNull(xmlString);
        return xmlString.Replace("\n", "\\n").Replace("\"", "\\\"");
    }

    public static string DeMaskFromJson(string xmlString)
    {
        ArgumentNullException.ThrowIfNull(xmlString);
        return xmlString.Replace("\\n", "\n").Replace("\\\"", "\"");
    }
}
