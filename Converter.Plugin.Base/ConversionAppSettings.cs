namespace Converter.Plugin.Base;

public class ConversionAppSettings
{
    private readonly Dictionary<string, string> appsettings;

    public ConversionAppSettings()
    {
        appsettings = new Dictionary<string, string>();
    }

    public ConversionAppSettings(Dictionary<string, string> appsettings)
    {
        this.appsettings = appsettings;
    }

    public string GetAppSetting(string Key, string? defaultValue = null)
    {
        if (defaultValue == null && !appsettings.ContainsKey(Key))
        {
            throw new Exception($@"Key ""{Key}"" nicht gefunden");
        }

        return appsettings[Key] ?? defaultValue ?? "";
    }
}
