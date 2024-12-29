using System.Xml;
using Microsoft.XmlDiffPatch;

namespace TaskConverter.Plugin.GTD.Utils;

public static class XmlUtil
{
    public static (bool hasError, string xmlDiff) DiffXml(XmlDocument originalXml, XmlDocument recreatedXml)
    {
        var xmldiff = new XmlDiff();
        var stream = new MemoryStream();
        var xmlWriter = XmlWriter.Create(stream);
        var hasError = !xmldiff.Compare(originalXml, recreatedXml, xmlWriter);
        xmlWriter.Flush();
        stream.Position = 0;
        var reader = new StreamReader(stream);
        var xmlDiff = reader.ReadToEnd();
        return (hasError, xmlDiff);
    }

    public static string MaskForJson(string xmlString)
    {
        return xmlString.Replace("\n", "\\n").Replace("\"", "\\\"");
    }

    public static string DeMaskFromJson(string xmlString)
    {
        return xmlString.Replace("\\n", "\n").Replace("\\\"", "\"");
    }
}
