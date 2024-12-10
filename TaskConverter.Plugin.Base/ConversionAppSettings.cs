using System.IO.Abstractions;

namespace TaskConverter.Plugin.Base;

public class ConversionAppData(IFileSystem fileSystem, Dictionary<string, string> appsettings)
{
    public IFileSystem FileSystem { get; } = fileSystem;
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
