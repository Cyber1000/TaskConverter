using System.Xml;
using Microsoft.XmlDiffPatch;

namespace Converter.Core.Utils
{
    public static class XmlUtil
    {
        public static (bool compareResult, string xmlDiff) DiffXml(XmlDocument originalXml, XmlDocument recreatedXml)
        {
            var xmldiff = new XmlDiff();
            var stream = new MemoryStream();
            var xmlWriter = XmlWriter.Create(stream);
            var compareResult = xmldiff.Compare(originalXml, recreatedXml, xmlWriter);
            var reader = new StreamReader(stream);
            var xmlDiff = reader.ReadToEnd();
            return (compareResult, xmlDiff);
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
}
