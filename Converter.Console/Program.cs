using Converter.Console;
using Converter.Core;
using Converter.Core.Utils;
using NodaTime;

enum Command
{
    CheckFile,
    CanMap
}

class Programm
{
    /// <summary>
    /// Command to parse data from DGT GTD
    /// </summary>
    /// <param name="commandType">Execute different commands</param>
    /// <param name="file">File to read</param>
    static int Main(Command commandType, FileInfo file)
    {
        TextWriter errorWriter = Console.Error;
        if (file is null || !file.Exists)
        {
            var info = file is null ? "Filename is mandatory." : $"File {file.FullName} not valid.";
            errorWriter.WriteLine(info);
            return 1;
        }
        var jsonReader = new JsonConfigurationReader(file);
        switch (commandType)
        {
            case Command.CheckFile:
            {
                CheckFile(jsonReader, errorWriter);
                break;
            }

            case Command.CanMap:
                CanMap(jsonReader, errorWriter);
                break;
        }
        return 0;
    }

    private static void CanMap(JsonConfigurationReader jsonReader, TextWriter errorConsole)
    {
        var taskInfo = jsonReader.TaskInfo;
        if (taskInfo == null)
        {
            errorConsole.WriteLine("There are no tasks in this file!");
            return;
        }
        try
        {
            var clock = SystemClock.Instance;
            var converterDateTimeZoneProvider = new ConverterDateTimeZoneProvider();
            var converter = new Converter.Core.Mapper.Converter(clock, converterDateTimeZoneProvider);
            converter.MapToModel(taskInfo);
            Console.WriteLine("File can be mapped to intermediate format!");
        }
        catch (Exception ex)
        {
            errorConsole.WriteLine($"Error while mapping to intermediate format: {ex.Message}");
        }
    }

    private static void CheckFile(JsonConfigurationReader jsonReader, TextWriter errorConsole)
    {
        try
        {
            var (isError, jsonDiff, xmlDiff) = jsonReader.Validate();
            if (isError)
            {
                errorConsole.WriteLine($"Output not equal to input - data doesn't match!");
                if (jsonDiff != null)
                    errorConsole.WriteLine($"Json: {jsonDiff}");
                if (!string.IsNullOrEmpty(xmlDiff))
                    errorConsole.WriteLine($"Xml: {xmlDiff}");
            }
            else
            {
                Console.WriteLine("Filecheck successful!");
            }
        }
        catch (Exception ex)
        {
            errorConsole.WriteLine($"Error in validating: {ex.Message}");
        }
    }
}
