using Converter.Core;

enum Command
{
    CheckFile,
    Convert
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
        //TODO HH: improvement - JsonConfigurationReader rausziehen? - wird in Checkfile und Convert gebraucht
        if (file is null || !file.Exists)
        {
            var info = file is null ? "Filename is mandatory." : $"File {file.FullName} not valid.";
            TextWriter errorWriter = Console.Error;
            errorWriter.WriteLine(info);
            return 1;
        }
        switch (commandType)
        {
            case Command.CheckFile:
            {
                CheckFile(file);
                break;
            }

            case Command.Convert:
                Convert(file);
                break;
        }
        return 0;
    }

    private static void Convert(FileInfo fileInfo)
    {
        var jsonReader = new JsonConfigurationReader(fileInfo);
        var taskInfo = jsonReader.TaskInfo;
        if (taskInfo == null)
            return;
        var mappedItems = Converter.Core.Mapper.Converter.MapToModel(taskInfo);
        //TODO HH: improvement - Fehlende Mappings erkennen
    }

    private static void CheckFile(FileInfo fileInfo)
    {
        var jsonReader = new JsonConfigurationReader(fileInfo);
        try
        {
            var (isError, jsonDiff, xmlDiff) = jsonReader.Validate();
            if (isError)
            {
                Console.WriteLine($"Output not equal to input - data doesn't match!");
                if (jsonDiff != null)
                    Console.WriteLine($"Json: {jsonDiff}");
                if (!string.IsNullOrEmpty(xmlDiff))
                    Console.WriteLine($"Xml: {xmlDiff}");
            }
            else
            {
                Console.WriteLine("Filecheck successful!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in validating: {ex.Message}");
        }
    }
}
