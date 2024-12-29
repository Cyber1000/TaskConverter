using TaskConverter.Model.Model;

namespace TaskConverter.Plugin.Base;

public interface IConverterPlugin
{
    string Name { get; }

    bool SetLocation(string fromLocation);

    (ConversionResult result, Exception? exception) CanConvertToTaskAppDataModel();

    TaskAppDataModel? ConvertToTaskAppDataModel();

    public (bool isError, string validationError) ValidateSource();
}
