using System.IO.Abstractions;
using System.Text;
using FluentValidation;
using TaskConverter.Plugin.Base;
using TaskConverter.Plugin.Base.Utils;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Validators;

namespace TaskConverter.Plugin.GTD;

//TODO HH: own writer
public class JsonConfigurationReader : IReader<GTDDataModel?>
{
    private string? RawJsonString;
    public GTDDataModel? Result { get; private set; }
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

        Result = _jsonConfigurationSerializer.Deserialize<GTDDataModel>(jsonString);
        if (Result == null)
            return;

        var validator = new GTDDataModelValidator();
        var dataConsistencyResult = validator.Validate(Result);

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
        if (Result == null)
            return null;

        return _jsonConfigurationSerializer.Serialize(Result);
    }

    public (bool isError, string validationError) CheckSource()
    {
        var roundtripValidator = new GTDRoundtripValidator(RawJsonString, Result, GetJsonOutput);
        return roundtripValidator.Validate();
    }
}
