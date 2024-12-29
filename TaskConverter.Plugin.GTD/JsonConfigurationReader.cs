using System.IO.Abstractions;
using System.Text;
using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Xml;
using FluentValidation;
using TaskConverter.Commons.Utils;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Utils;
using TaskConverter.Plugin.GTD.Validators;

namespace TaskConverter.Plugin.GTD;

public class JsonConfigurationReader
{
    private string? RawJsonString;
    public GTDDataModel? TaskInfo { get; private set; }
    private readonly IFileSystem _fileSystem;
    private readonly IJsonConfigurationSerializer _jsonConfigurationSerializer;

    public JsonConfigurationReader(IFileInfo inputFile, IFileSystem fileSystem, IJsonConfigurationSerializer jsonConfigurationSerializer)
    {
        _fileSystem = fileSystem;
        _jsonConfigurationSerializer = jsonConfigurationSerializer;
        if (!inputFile.Exists)
            throw new Exception($"File {inputFile} doesn't exist");

        var jsonString = inputFile.FullName.EndsWith(".zip") ? inputFile.ReadFromZip() : fileSystem.File.ReadAllText(inputFile.FullName);
        Read(jsonString);
    }

    private void Read(string jsonString)
    {
        RawJsonString = jsonString;

        TaskInfo = _jsonConfigurationSerializer.Deserialize<GTDDataModel>(jsonString);
        if (TaskInfo == null)
            return;

        var validator = new GTDDataModelValidator();
        var validationResult = validator.Validate(TaskInfo);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
    }

    public void Write(IFileInfo outputFile)
    {
        var output = GetJsonOutput();
        if (output == null)
            return;

        var fileName = outputFile.FullName;
        _fileSystem.File.WriteAllText(fileName, output, Encoding.UTF8);
    }

    public string? GetJsonOutput()
    {
        if (TaskInfo == null)
            return null;

        return _jsonConfigurationSerializer.Serialize(TaskInfo);
    }

    public (bool isError, string validationError) Validate()
    {
        if (TaskInfo == null)
            return (false, "");

        var recreatedJsonText = GetJsonOutput();
        var jsonDiff = JsonDiffPatcher.Diff(JsonUtil.NormalizeText(RawJsonString), recreatedJsonText, new JsonDeltaFormatter(), JsonUtil.GetDiffOptions());
        var jsonError = jsonDiff is null || (jsonDiff is JsonArray jsonArray && jsonArray.Count > 0);

        XmlDocument originalXml = ParseXmlFromString(RawJsonString);
        var recreatedXml = string.IsNullOrEmpty(recreatedJsonText) ? new XmlDocument() : ParseXmlFromString(recreatedJsonText);
        var (compareResult, xmlDiff) = XmlUtil.DiffXml(originalXml, recreatedXml);
        var xmlError = !compareResult;
        var validationError = "";
        if (jsonError)
            validationError += $"Json: {jsonDiff}{Environment.NewLine}";
        if (xmlError)
            validationError += $"Xml: {xmlDiff}";
        return (jsonError || xmlError, validationError);

        static XmlDocument ParseXmlFromString(string jsonString)
        {
            var originalXmlPattern = @"com\.dg\.gtd\.android\.lite_preferences"":\s*""([^""\\]*(?:\\.[^""\\]*)*)";
            var match = Regex.Match(jsonString, originalXmlPattern);
            string originalXmlString;
            if (match.Success)
            {
                originalXmlString = match.Groups[1].Value;
            }
            else
            {
                throw new Exception("Couldn't find original Xml!");
            }

            XmlDocument originalXml = new();
            originalXml.LoadXml(XmlUtil.DeMaskFromJson(originalXmlString));
            return originalXml;
        }
    }
}
