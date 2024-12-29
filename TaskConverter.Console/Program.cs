using TaskConverter.Console;
using TaskConverter.Console.PluginHandling;
using TaskConverter.Plugin.Base;

enum Command
{
    CheckSource,
    CanMap,
}

class Programm
{
    /// <summary>
    /// Command to map data between different todo/planning apps
    /// </summary>
    /// <param name="commandType">Execute different commands</param>
    /// <param name="fromModel">Convert from Model</param>
    /// <param name="fromLocation">File or Url to interact</param>
    static int Main(Command commandType, string fromModel, string fromLocation)
    {
        //TODO HH: Maybe we can change fromModel from string to some dynamic enum (for better help)
        TextWriter errorWriter = Console.Error;
        fromModel = fromModel?.ToLowerInvariant() ?? "";

        var commands = LoadPluginsAndGetCommands();
        if (string.IsNullOrEmpty(fromModel) || !commands.ContainsKey(fromModel))
        {
            //TODO HH: better message, when there are no plugins
            errorWriter.WriteLine($"FromModel is mandatory and must be a valid plugin. Valid plugins are: {string.Join(',', GetAvailablePlugins(commands))}");
            return 1;
        }
        var fromCommand = commands[fromModel];
        if (string.IsNullOrEmpty(fromLocation) || !fromCommand.SetLocation(fromLocation))
        {
            errorWriter.WriteLine("FromLocation is mandatory and must be a valid location.");
            return 1;
        }

        switch (commandType)
        {
            case Command.CheckSource:
            {
                CheckSource(fromCommand, errorWriter);
                break;
            }

            case Command.CanMap:
                CanMap(fromCommand, errorWriter);
                break;
        }
        return 0;
    }

    private static List<string> GetAvailablePlugins(IDictionary<string, IConverterPlugin> commands) => commands.Select(c => c.Key).ToList();

    private static Dictionary<string, IConverterPlugin> LoadPluginsAndGetCommands()
    {
        var pluginBaseDir = Path.Combine(AppContext.BaseDirectory, "plugins");
        if (!Directory.Exists(pluginBaseDir))
        {
            return [];
        }
        var pluginLoader = new PluginHandler(pluginBaseDir);
        return pluginLoader.GetAllCommands<IConverterPlugin>(SettingsHelper.GetAppSettings()).ToDictionary(c => c.Name.ToLowerInvariant(), c => c);
    }

    private static void CanMap(IConverterPlugin command, TextWriter errorConsole)
    {
        var (result, exception) = command.CanConvertToTaskAppDataModel();
        switch (result)
        {
            case ConversionResult.CanConvert:
                Console.WriteLine("File can be mapped to intermediate format.");
                break;
            case ConversionResult.ConversionError:
                errorConsole.WriteLine($"Error while mapping to intermediate format;{exception}");
                break;
            case ConversionResult.NoTasks:
                errorConsole.WriteLine("There are no tasks in this file!");
                break;
        }
    }

    private static void CheckSource(IConverterPlugin command, TextWriter errorConsole)
    {
        try
        {
            var (isError, validationError) = command.ValidateSource();
            if (isError)
            {
                errorConsole.WriteLine($"Output not equal to input - data doesn't match:{System.Environment.NewLine}{validationError}");
            }
            else
            {
                Console.WriteLine("Validation successful!");
            }
        }
        catch (Exception ex)
        {
            errorConsole.WriteLine($"Error in validating: {ex.Message}");
        }
    }
}
