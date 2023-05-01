using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Xml;
using NodaTime;
using TaskConverter.Commons.Utils;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.ConversionHelper;
using TaskConverter.Commons.ConversionHelper;
using TaskConverter.Plugin.GTD.Utils;
using System.Text.Json.JsonDiffPatch;

namespace TaskConverter.Plugin.GTD;

public class JsonConfigurationReader
{
    private string? RawJsonString;
    public TaskInfo? TaskInfo { get; private set; }

    public JsonConfigurationReader(FileInfo inputFile)
    {
        if (!inputFile.Exists)
            throw new Exception($"File {inputFile} doesn't exist");

        var jsonString = inputFile.FullName.EndsWith(".zip") ? inputFile.ReadFromZip() : File.ReadAllText(inputFile.FullName);
        Read(jsonString);
    }

    public JsonConfigurationReader(string jsonString)
    {
        Read(jsonString);
    }

    private void Read(string jsonString)
    {
        RawJsonString = jsonString;
        JsonSerializerOptions options = InitSerializerOptions();

        TaskInfo = JsonSerializer.Deserialize<TaskInfo>(jsonString, options);
        AssertValues(TaskInfo);
    }

    private static JsonSerializerOptions InitSerializerOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = new TaskInfoJsonNamingPolicy(),
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters =
            {
                new ExactLocalDateTimeConverter<LocalDateTime?>(),
                new ExactLocalDateTimeConverter<LocalDateTime>(),
                new TaskInfoBoolConverter()
            }
        };

        return options;
    }

    private static void AssertValues(TaskInfo? taskInfo)
    {
        if (taskInfo == null)
            return;
        string? errorText = null;
        foreach (var singleTaskInfo in taskInfo.GetAllEntries)
        {
            if (!string.IsNullOrEmpty(singleTaskInfo.Uuid))
                errorText = "Only empty Uuids are accepted";
            if (string.IsNullOrEmpty(singleTaskInfo.Title))
                errorText = "Title must not be null";

            switch (singleTaskInfo)
            {
                case TaskInfoTaskEntry taskInfoTaskEntry:
                    if (taskInfoTaskEntry.StartDate is not null)
                        errorText = "Start Date not implemented";
                    if (taskInfoTaskEntry.StartTimeSet)
                        errorText = "Start Time Set not implemented";
                    if (
                        taskInfoTaskEntry.DueDateModifier != DueDateModifier.DueBy
                        && taskInfoTaskEntry.DueDateModifier != DueDateModifier.OptionallyOn
                    )
                        errorText = $"Due date {taskInfoTaskEntry.DueDateModifier} not implemented";
                    if (taskInfoTaskEntry.Duration > 0)
                        errorText = "Duration not implemented";
                    if (taskInfoTaskEntry.Goal > 0)
                        errorText = "Goal not implemented";
                    if (!string.IsNullOrEmpty(taskInfoTaskEntry.TrashBin))
                        errorText = "TrashBin not implemented";
                    if (taskInfoTaskEntry.Importance > 0)
                        errorText = "Importance not implemented";
                    if (!string.IsNullOrEmpty(taskInfoTaskEntry.MetaInformation))
                        errorText = "MetaInformation not implemented";
                    if (taskInfoTaskEntry.Hide != Hide.DontHide && taskInfoTaskEntry.Hide != Hide.SixMonthsBeforeDue)
                        errorText = $"Hide not implemented with value {taskInfoTaskEntry.Hide}";
                    break;
                case TaskInfoTaskNote:
                    errorText = "TaskInfoTaskNote not implemented";
                    break;
            }

            if (taskInfo.Preferences == null)
            {
                errorText = "Preferences should have entries";
            }

            if (errorText is not null)
            {
                throw new NotImplementedException($"Item {singleTaskInfo.GetType()} with Id {singleTaskInfo.Id}: {errorText}");
            }
        }
    }

    public void Write(FileInfo outputFile)
    {
        var output = GetJsonOutput();
        File.WriteAllText(outputFile.FullName, output, Encoding.UTF8);
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
        var jsonDiff = JsonDiffPatcher.Diff(
            JsonUtil.NormalizeText(RawJsonString),
            recreatedJsonText,
            new JsonDeltaFormatter(),
            JsonUtil.GetDiffOptions()
        );
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
