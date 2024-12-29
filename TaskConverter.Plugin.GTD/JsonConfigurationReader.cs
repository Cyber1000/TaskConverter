using System.IO.Abstractions;
using System.Text;
using FluentValidation;
using TaskConverter.Commons.Utils;
using TaskConverter.Plugin.GTD.Model;
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
        var dataConsistencyResult = validator.Validate(TaskInfo);

        if (!dataConsistencyResult.IsValid)
        {
            throw new ValidationException(dataConsistencyResult.Errors);
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

    private string? GetJsonOutput()
    {
        if (TaskInfo == null)
            return null;

        return _jsonConfigurationSerializer.Serialize(TaskInfo);
    }

    public (bool isError, string validationError) ValidateRoundtrip()
    {
        var roundtripValidator = new GTDRoundtripValidator(RawJsonString, TaskInfo, GetJsonOutput);
        return roundtripValidator.Validate();
    }
}
