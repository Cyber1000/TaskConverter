using TaskConverter.Model.Model;

namespace TaskConverter.Plugin.Base;

public interface IConverterPlugin
{
    string Name { get; }

    bool SetLocation(string fromLocation);

    (bool?, Exception? exception) CanConvertToTaskAppDataModel();

    TaskAppDataModel? ConvertToTaskAppDataModel();

    public (bool isError, string validationError) ValidateSource();
}
