using Converter.Commons.Utils;
using Converter.Model.Model;

namespace Converter.Plugin.Base;

public interface IConverterPlugin
{
    string Name { get; }

    bool SetLocation(string fromLocation);

    (bool?, Exception? exception) CanConvertToTaskInfoModel();

    TaskInfoModel? ConvertToTaskInfoModel();

    public (bool isError, string validationError) ValidateSource();
}
