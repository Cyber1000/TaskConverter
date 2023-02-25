using Converter.Core;

enum Command
{
    CheckFile
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
        if (commandType == Command.CheckFile)
        {
            if (file is null || !file.Exists)
            {
                var info = file is null ? "Filename is mandatory." : $"File {file.FullName} not valid.";
                TextWriter errorWriter = Console.Error;
                errorWriter.WriteLine(info);
                return 1;
            }
            CheckFile(file);
        }
        return 0;
    }

    private static void CheckFile(FileInfo fileInfo)
    {
        var jsonReader = new JsonConfigurationReader();
        var inputFile = fileInfo;
        jsonReader.Read(inputFile);
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