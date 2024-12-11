namespace TaskConverter.Plugin.Base;

public class ConversionAppSettings(Dictionary<string, string> appsettings)
{
    private readonly Dictionary<string, string> appsettings = appsettings;

    public string GetAppSetting(string Key, string? defaultValue = null)
    {
        if (defaultValue == null && !appsettings.ContainsKey(Key))
        {
            throw new Exception($@"Key ""{Key}"" nicht gefunden");
        }

        return appsettings[Key] ?? defaultValue ?? "";
    }
}
