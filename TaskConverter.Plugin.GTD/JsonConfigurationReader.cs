using System.IO.Abstractions;
using FluentValidation;
using TaskConverter.Commons;
using TaskConverter.Plugin.Base;
using TaskConverter.Plugin.Base.Utils;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Validators;

namespace TaskConverter.Plugin.GTD;

public class JsonConfigurationReader(IFileSystem FileSystem, IJsonConfigurationSerializer JsonConfigurationSerializer) : IReader<GTDDataModel?>
{
    public GTDDataModel? Read(string source)
    {
        return ReadInternal(source).dataModel;
    }

    private (string rawJsonString, GTDDataModel? dataModel) ReadInternal(string source)
    {
        var inputFile = FileSystem.FileInfo.New(source);

        if (!inputFile.Exists)
            throw new Exception($"File {inputFile} doesn't exist");

        var jsonString = inputFile.FullName.EndsWith(".zip") ? inputFile.ReadFromZip() : FileSystem.File.ReadAllText(inputFile.FullName);

        var rawJsonString = jsonString;

        var result = JsonConfigurationSerializer.Deserialize<GTDDataModel>(jsonString);
        if (result == null)
            return (rawJsonString, null);

        var validator = new GTDDataModelValidator();
        var dataConsistencyResult = validator.Validate(result);

        if (!dataConsistencyResult.IsValid)
        {
            throw new ValidationException(dataConsistencyResult.Errors);
        }
        return (rawJsonString, result);
    }

    private string? GetJsonOutput(GTDDataModel? result)
    {
        if (result == null)
            return null;

        return JsonConfigurationSerializer.Serialize(result);
    }

    public SourceResult CheckSource(string source)
    {
        var (rawJsonString, result) = ReadInternal(source);

        var roundtripValidator = new GTDRoundtripValidator(rawJsonString, result, GetJsonOutput(result));
        return roundtripValidator.Validate();
    }
}
