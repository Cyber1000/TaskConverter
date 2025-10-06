namespace TaskConverter.Commons;

public interface IConversionAppSettings
{
    T GetAppSetting<T>(string key, T? defaultValue = default);
}
