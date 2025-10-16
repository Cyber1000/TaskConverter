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
        var commandTypeOption = new Option<Command>("--command-type") { Description="Execute different commands" };
        var fromModelOption = new Option<string>("--from-model") { Required = true };
        var toModelOption = new Option<string>("--to-model") { Required = false };
        var fromLocationOption = new Option<string>("--from-location") { Description="File or Url to interact", Required = true };
        var toLocationOption = new Option<string>("--to-location") { Description="File or Url to interact", Required = false };

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

            ExecuteCommand(commandType, fromCommand!, toCommand, fromLocation, toLocation, errorWriter);
            return 0;
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

    private static void ExecuteCommand(Command commandType, IConverterPlugin fromCommand, IConverterPlugin? toCommand, string fromLocation, string? toLocation, TextWriter errorWriter)
    {
        switch (commandType)
        {
            case Command.CheckSource:
                CheckSource(fromCommand, fromLocation, errorWriter);
                break;
            case Command.CanMap:
                CanMap(fromCommand, fromLocation, errorWriter);
                break;
            case Command.Map:
                Map(fromCommand, toCommand!, fromLocation, toLocation!, errorWriter);
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

    private static void CanMap(IConverterPlugin command, string source, TextWriter errorWriter)
    {
        var conversionResultStatus = command.CanConvertToIntermediateFormat(source);
        CheckMapping(errorWriter, conversionResultStatus.Success, conversionResultStatus.ResultType, conversionResultStatus.Exception);
    }

    private static void CheckSource(IConverterPlugin command, string source, TextWriter errorWriter)
    {
        try
        {
            var (isError, validationError) = command.CheckSource(source);
            if (isError)
            {
                errorWriter.WriteLine($"Output not equal to input - data doesn't match:{Environment.NewLine}{validationError?.Message}");
            }
            else
            {
                Console.WriteLine("Validation successful!");
            }
        }
        catch (Exception ex)
        {
            errorWriter.WriteLine($"Error in validating: {ex.Message}");
        }
    }

    private static void Map(IConverterPlugin fromCommand, IConverterPlugin toCommand, string fromLocation, string toLocation, TextWriter errorWriter)
    {
        var (success, resultType, sourceModel, exception) = fromCommand.ConvertToIntermediateFormat(fromLocation);
        if (CheckMapping(errorWriter, success, resultType, exception))
        {
            var result = toCommand.ConvertFromIntermediateFormat(toLocation, sourceModel!);
            if (!result.Success)
            {
                switch (result.ResultType)
                {
                    case ConversionResultType.WriterError:
                        errorWriter.WriteLine("Error with writing the destination.");
                        break;
                    case ConversionResultType.ConversionError:
                        errorWriter.WriteLine($"Error while mapping from intermediate format;{result.Exception}");
                        break;
                }
            }
        }
    }

    private static bool CheckMapping(TextWriter errorWriter, bool success, ConversionResultType conversionResultType, Exception? exception)
    {
        if (success)
            return true;

        switch (conversionResultType)
        {
            case ConversionResultType.ReaderError:
                Console.WriteLine("Error with reading the source.");
                break;
            case ConversionResultType.CanConvert:
                Console.WriteLine("File can be mapped to intermediate format.");
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
