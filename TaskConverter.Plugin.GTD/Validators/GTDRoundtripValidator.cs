using System.Text;
using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Xml;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Utils;

namespace TaskConverter.Plugin.GTD.Validators
{
    public partial class GTDRoundtripValidator
    {
        private readonly string? _rawJsonString;
        private readonly GTDDataModel? _taskInfo;
        private readonly Func<string?> _getJsonOutput;

        public GTDRoundtripValidator(string? rawJsonString, GTDDataModel? taskInfo, Func<string?> getJsonOutput)
        {
            _rawJsonString = rawJsonString;
            _taskInfo = taskInfo;
            _getJsonOutput = getJsonOutput;
        }

        [GeneratedRegex(@"com\.dg\.gtd\.android\.lite_preferences"":\s*""([^""\\]*(?:\\.[^""\\]*)*)")]
        private static partial Regex PreferencesRegex();

        public (bool isError, string validationError) Validate()
        {
            if (_rawJsonString == null || _taskInfo == null)
                return (false, "");

            var recreatedJsonText = _getJsonOutput();
            var (jsonError, jsonDiffText) = ValidateJson(_rawJsonString, recreatedJsonText);
            var (xmlError, xmlDiffText) = ValidateXml(_rawJsonString, recreatedJsonText);

            var validationError = BuildValidationMessage(jsonError, jsonDiffText, xmlError, xmlDiffText);
            return (jsonError || xmlError, validationError);
        }

        private (bool hasError, string diffText) ValidateJson(string original, string? recreated)
        {
            var jsonDiff = JsonDiffPatcher.Diff(JsonUtil.NormalizeText(original), recreated, new JsonDeltaFormatter(), JsonUtil.GetDiffOptions());

            var hasError = jsonDiff is null || (jsonDiff is JsonArray arr && arr.Count > 0);
            return (hasError, jsonDiff?.ToString() ?? string.Empty);
        }

        private (bool hasError, string diffText) ValidateXml(string original, string? recreated)
        {
            var originalXml = ParseXmlFromString(original);
            var recreatedXml = string.IsNullOrEmpty(recreated) ? new XmlDocument() : ParseXmlFromString(recreated);

            return XmlUtil.DiffXml(originalXml, recreatedXml);
        }

        private string BuildValidationMessage(bool jsonError, string jsonDiff, bool xmlError, string xmlDiff)
        {
            var sb = new StringBuilder();
            if (jsonError)
                sb.AppendLine($"Json: {jsonDiff}");
            if (xmlError)
                sb.Append($"Xml: {xmlDiff}");
            return sb.ToString();
        }

        private static XmlDocument ParseXmlFromString(string jsonString)
        {
            var match = PreferencesRegex().Match(jsonString);
            if (!match.Success)
            {
                throw new Exception("XML preferences section not found in JSON input.");
            }

            var xmlString = XmlUtil.DeMaskFromJson(match.Groups[1].Value);
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlString);

            return xmlDocument;
        }
    }
}
