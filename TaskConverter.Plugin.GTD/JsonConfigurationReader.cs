using System.IO.Abstractions;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Xml;
using FluentValidation;
using NodaTime;
using TaskConverter.Commons.ConversionHelper;
using TaskConverter.Commons.Utils;
using TaskConverter.Plugin.GTD.ConversionHelper;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Utils;
using TaskConverter.Plugin.GTD.Validators;

namespace TaskConverter.Plugin.GTD;

public class JsonConfigurationReader
{
    private string? RawJsonString;
    public GTDDataModel? TaskInfo { get; private set; }
    private IFileSystem FileSystem { get; }

    public JsonConfigurationReader(IFileInfo inputFile, IFileSystem fileSystem)
    {
        FileSystem = fileSystem;
        if (!inputFile.Exists)
            throw new Exception($"File {inputFile} doesn't exist");

        var jsonString = inputFile.FullName.EndsWith(".zip") ? inputFile.ReadFromZip() : fileSystem.File.ReadAllText(inputFile.FullName);
        Read(jsonString);
    }

    public JsonConfigurationReader(string jsonString)
    {
        FileSystem = new FileSystem();
        Read(jsonString);
    }

    private void Read(string jsonString)
    {
        RawJsonString = jsonString;
        JsonSerializerOptions options = InitSerializerOptions();

        TaskInfo = JsonSerializer.Deserialize<GTDDataModel>(jsonString, options);
        if (TaskInfo == null)
            return;

        var validator = new GTDDataModelValidator();
        var validationResult = validator.Validate(TaskInfo);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
    }

    private static JsonSerializerOptions InitSerializerOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = new TaskInfoJsonNamingPolicy(),
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new ExactLocalDateTimeConverter<LocalDateTime?>(), new ExactLocalDateTimeConverter<LocalDateTime>(), new TaskInfoBoolConverter() },
        };

        return options;
    }

    public void Write(IFileInfo outputFile)
    {
        var output = GetJsonOutput();
        if (output == null)
            return;

        var fileName = outputFile.FullName;
        FileSystem.File.WriteAllText(fileName, output, Encoding.UTF8);
    }

    public string? GetJsonOutput()
    {
        if (TaskInfo == null)
            return null;

        JsonSerializerOptions options = InitSerializerOptions();
        return JsonSerializer.Serialize(TaskInfo, options);
    }

    public (bool isError, string validationError) Validate()
    {
        if (TaskInfo == null || RawJsonString == null)
            throw new Exception("Use Read before Validating!");

        var recreatedJsonText = GetJsonOutput();
        var jsonDiff = JsonDiffPatcher.Diff(JsonUtil.NormalizeText(RawJsonString), recreatedJsonText, new JsonDeltaFormatter(), JsonUtil.GetDiffOptions());
        var isError = jsonDiff is null || (jsonDiff is JsonArray jsonArray && jsonArray.Count > 0);

        XmlDocument originalXml = ParseOriginalXml();
        var recreatedXml = TaskInfo!.Preferences![0].XmlConfig;
        var (compareResult, xmlDiff) = XmlUtil.DiffXml(originalXml, recreatedXml!);
        isError = isError || !compareResult;
        var validationError = "";
        if (jsonDiff != null)
            validationError += $"Json: {jsonDiff}{System.Environment.NewLine}";
        if (!string.IsNullOrEmpty(xmlDiff))
            validationError += $"Xml: {xmlDiff}";
        return (isError, validationError);

        XmlDocument ParseOriginalXml()
        {
            var originalXmlPattern = @"com\.dg\.gtd\.android\.lite_preferences"":\s*""([^""\\]*(?:\\.[^""\\]*)*)";
            var match = Regex.Match(RawJsonString, originalXmlPattern);
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
