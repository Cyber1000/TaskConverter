using TaskConverter.Model.Model;

namespace TaskConverter.Plugin.Base;

public interface IConverterPlugin
{
    string Name { get; }

    bool SetLocation(string fromLocation);

    (bool?, Exception? exception) CanConvertToTaskInfoModel();

    TaskInfoModel? ConvertToTaskInfoModel();

    public (bool isError, string validationError) ValidateSource();
}
