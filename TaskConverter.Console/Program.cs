using System.CommandLine;
using System.Data;
using TaskConverter.Commons;
using TaskConverter.Console;
using TaskConverter.Console.PluginHandling;

enum Command
{
    CheckSource,
    CanMap,
    Map,
}

class Programm
{
    static async Task<int> Main(string[] args)
    {
        var commandTypeOption = new Option<Command>("--command-type") { Description = "Execute different commands" };
        var fromModelOption = new Option<string>("--from-model") { Required = true };
        var toModelOption = new Option<string>("--to-model") { Required = false };
        var fromLocationOption = new Option<string>("--from-location") { Description = "File or Url to interact", Required = true };
        var toLocationOption = new Option<string>("--to-location") { Description = "File or Url to interact", Required = false };

        var rootCommand = new RootCommand("Command to map data between different todo/planning apps") { commandTypeOption, fromModelOption, toModelOption, fromLocationOption, toLocationOption };

        var commands = LoadPluginsAndGetCommands();
        fromModelOption.Description = $"Convert from Model. Valid plugins: {string.Join(", ", GetAvailablePlugins(commands))}";
        toModelOption.Description = $"Convert to Model. Valid plugins: {string.Join(", ", GetAvailablePlugins(commands))}";

        rootCommand.SetAction(parseResult =>
        {
            var commandType = parseResult.GetValue(commandTypeOption);
            var fromModel = parseResult.GetValue(fromModelOption)?.ToLowerInvariant() ?? string.Empty;
            var toModel = parseResult.GetValue(toModelOption)?.ToLowerInvariant() ?? string.Empty;
            var fromLocation = parseResult.GetValue(fromLocationOption) ?? string.Empty;
            var toLocation = parseResult.GetValue(toLocationOption) ?? string.Empty;

            var errorWriter = Console.Error;

            if (!TryGetModel(fromModel, commands, errorWriter, "FromModel", out var fromCommand))
                return 1;

            IConverterPlugin? toCommand = null;
            if (commandType == Command.Map && !TryGetModel(toModel, commands, errorWriter, "ToModel", out toCommand))
                return 1;

            if (!ValidateLocation(fromLocation, errorWriter, "FromLocation"))
                return 1;

            if (commandType == Command.Map && !ValidateLocation(toLocation, errorWriter, "ToLocation"))
                return 1;

            return ExecuteCommand(commandType, fromCommand!, toCommand, fromLocation, toLocation, errorWriter) ? 0 : 1;
        });

        var parseResult = rootCommand.Parse(args);
        return await parseResult.InvokeAsync();
    }

    private static bool TryGetModel(string model, Dictionary<string, IConverterPlugin> commands, TextWriter errorWriter, string modelName, out IConverterPlugin? converterPlugin)
    {
        converterPlugin = null;
        if (string.IsNullOrEmpty(model) || !commands.TryGetValue(model, out converterPlugin))
        {
            var availablePlugins = GetAvailablePlugins(commands);
            if (availablePlugins.Count == 0)
                errorWriter.WriteLine("There are no valid plugins.");
            else
                errorWriter.WriteLine($"{modelName} is mandatory and must be a valid plugin. Valid plugins are: {string.Join(',', availablePlugins)}");
            return false;
        }
        return true;
    }

    private static bool ValidateLocation(string location, TextWriter errorWriter, string modelName)
    {
        if (string.IsNullOrEmpty(location))
        {
            errorWriter.WriteLine($"{modelName} is mandatory and must be a valid location.");
            return false;
        }
        return true;
    }

    private static bool ExecuteCommand(Command commandType, IConverterPlugin fromCommand, IConverterPlugin? toCommand, string fromLocation, string? toLocation, TextWriter errorWriter)
    {
        return commandType switch
        {
            Command.CheckSource => CheckSource(fromCommand, fromLocation, errorWriter),
            Command.CanMap => CanMap(fromCommand, fromLocation, errorWriter),
            Command.Map => Map(fromCommand, toCommand!, fromLocation, toLocation!, errorWriter),
            _ => throw new ArgumentOutOfRangeException(nameof(commandType), commandType, "Unknown command."),
        };
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

    private static bool CanMap(IConverterPlugin command, string source, TextWriter errorWriter)
    {
        var conversionResultStatus = command.CanConvertToIntermediateFormat(source);
        if (!CheckMapping(errorWriter, conversionResultStatus.Success, conversionResultStatus.ResultType, conversionResultStatus.Exception))
            return false;

        Console.WriteLine("Source can be mapped to the intermediate format.");
        return true;
    }

    private static bool CheckSource(IConverterPlugin command, string source, TextWriter errorWriter)
    {
        var (isSuccess, validationError) = command.CheckSource(source);
        if (isSuccess)
        {
            Console.WriteLine("Validation successful!");
            return true;
        }

        errorWriter.WriteLine($"Errors on checking source:{Environment.NewLine}{validationError?.Message}");
        return false;
    }

    private static bool Map(IConverterPlugin fromCommand, IConverterPlugin toCommand, string fromLocation, string toLocation, TextWriter errorWriter)
    {
        var (success, resultType, sourceModel, exception) = fromCommand.ConvertToIntermediateFormat(fromLocation);
        if (!CheckMapping(errorWriter, success, resultType, exception))
            return false;

        var result = toCommand.ConvertFromIntermediateFormat(toLocation, sourceModel!);
        if (result.Success)
        {
            Console.WriteLine($"Mapped {fromCommand.Name} to {toCommand.Name}.");
            return true;
        }

        switch (result.ResultType)
        {
            case ConversionResultType.WriterError:
                errorWriter.WriteLine("Error with writing the destination.");
                break;
            case ConversionResultType.ConversionError:
                errorWriter.WriteLine($"Error while mapping from intermediate format;{result.Exception}");
                break;
        }
        return false;
    }

    private static bool CheckMapping(TextWriter errorWriter, bool success, ConversionResultType conversionResultType, Exception? exception)
    {
        if (success)
            return true;

        switch (conversionResultType)
        {
            case ConversionResultType.ReaderError:
                errorWriter.WriteLine("Error with reading the source.");
                break;
            case ConversionResultType.ConversionError:
                errorWriter.WriteLine($"Error while mapping to intermediate format;{exception}");
                break;
            case ConversionResultType.NoTasks:
                errorWriter.WriteLine("There are no tasks in this file!");
                break;
        }
        return false;
    }
}
