using System.CommandLine;
using TaskConverter.Commons;
using TaskConverter.Console;
using TaskConverter.Console.PluginHandling;

enum Command
{
    CheckSource,
    CanMap,
}

class Programm
{
    static async Task<int> Main(string[] args)
    {
        var commandTypeOption = new Option<Command>("--command-type", "Execute different commands");
        var fromModelOption = new Option<string>("--from-model") { Required = true };
        var fromLocationOption = new Option<string>("--from-location", "File or Url to interact") { Required = true };

        var rootCommand = new RootCommand("Command to map data between different todo/planning apps") { commandTypeOption, fromModelOption, fromLocationOption };

        var commands = LoadPluginsAndGetCommands();
        fromModelOption.Description = $"Convert from Model. Valid plugins: {string.Join(", ", GetAvailablePlugins(commands))}";

        rootCommand.SetAction(parseResult =>
        {
            var command = parseResult.GetValue(commandTypeOption);
            var model = parseResult.GetValue(fromModelOption)?.ToLowerInvariant() ?? string.Empty;
            var location = parseResult.GetValue(fromLocationOption) ?? string.Empty;

            var errorWriter = Console.Error;

            if (!ValidateFromModel(model, commands, errorWriter))
                return 1;

            var fromCommand = commands[model];
            if (!ValidateFromLocation(location, fromCommand, errorWriter))
                return 1;

            ExecuteCommand(command, fromCommand, errorWriter);
            return 0;
        });

        var parseResult = rootCommand.Parse(args);
        return await parseResult.InvokeAsync();
    }

    private static bool ValidateFromModel(string fromModel, Dictionary<string, IConverterPlugin> commands, TextWriter errorWriter)
    {
        if (string.IsNullOrEmpty(fromModel) || !commands.TryGetValue(fromModel, out _))
        {
            var availablePlugins = GetAvailablePlugins(commands);
            if (availablePlugins.Count == 0)
                errorWriter.WriteLine("There are no valid plugins.");
            else
                errorWriter.WriteLine($"FromModel is mandatory and must be a valid plugin. Valid plugins are: {string.Join(',', availablePlugins)}");
            return false;
        }
        return true;
    }

    private static bool ValidateFromLocation(string fromLocation, IConverterPlugin command, TextWriter errorWriter)
    {
        if (string.IsNullOrEmpty(fromLocation) || !command.SetLocation(fromLocation))
        {
            errorWriter.WriteLine("FromLocation is mandatory and must be a valid location.");
            return false;
        }
        return true;
    }

    private static void ExecuteCommand(Command commandType, IConverterPlugin command, TextWriter errorWriter)
    {
        switch (commandType)
        {
            case Command.CheckSource:
                CheckSource(command, errorWriter);
                break;
            case Command.CanMap:
                CanMap(command, errorWriter);
                break;
        }
    }

    private static List<string> GetAvailablePlugins(IDictionary<string, IConverterPlugin> commands) => commands.Select(c => c.Key).ToList();

    private static Dictionary<string, IConverterPlugin> LoadPluginsAndGetCommands()
    {
        var pluginBaseDir = Path.Combine(AppContext.BaseDirectory, "plugins");
        if (!Directory.Exists(pluginBaseDir))
            return [];

        var pluginLoader = new PluginHandler(pluginBaseDir);
        return pluginLoader.GetAllCommands<IConverterPlugin>(SettingsHelper.GetAppSettings()).ToDictionary(c => c.Name.ToLowerInvariant(), c => c);
    }

    private static void CanMap(IConverterPlugin command, TextWriter errorConsole)
    {
        var (result, exception) = command.CanConvertToIntermediateFormat();
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
