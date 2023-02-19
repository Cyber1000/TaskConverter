using Converter.Core;

//TODO HH: current path?
//TODO HH: params for console
var jsonReader = new JsonConfigurationReader();
var inputFile = new FileInfo("../../GTD.json");
var outputFile = new FileInfo("../../GTD_new.json");
jsonReader.Read(inputFile);
jsonReader.Write(outputFile);
try
{
    var (isError, jsonDiff, xmlDiff) = jsonReader.Validate();
    if (isError)
    {
        //TODO HH: other text
        Console.WriteLine("Sucessful completed!");
    }
    else
    {
        Console.WriteLine($"Output not equal to input - data don't match!");
        if (jsonDiff != null)
            Console.WriteLine($"Json: {jsonDiff}");
        if (!string.IsNullOrEmpty(xmlDiff))
            Console.WriteLine($"Xml: {xmlDiff}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error in validating: {ex.Message}");
}
