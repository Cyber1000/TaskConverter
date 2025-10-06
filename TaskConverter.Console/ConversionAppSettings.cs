using TaskConverter.Commons;

namespace TaskConverter.Console;

public class ConversionAppSettings(Dictionary<string, string> appsettings) : IConversionAppSettings
{
    private readonly Dictionary<string, string> _appsettings = appsettings ?? throw new ArgumentNullException(nameof(appsettings));

    public T GetAppSetting<T>(string key, T? defaultValue = default)
    {
        if (!_appsettings.TryGetValue(key, out var rawValue) || string.IsNullOrWhiteSpace(rawValue))
        {
            if (defaultValue is not null)
                return defaultValue;

            throw new KeyNotFoundException($@"Key ""{key}"" not found in appsettings.");
        }

        try
        {
            var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            var converted = Convert.ChangeType(rawValue, targetType);
            return (T)converted!;
        }
        catch (Exception ex)
        {
            if (defaultValue is not null)
                return defaultValue;

            throw new InvalidCastException($@"Value ""{rawValue}"" for key ""{key}"" could not be converted to {typeof(T).Name}.", ex);
        }
    }
}
