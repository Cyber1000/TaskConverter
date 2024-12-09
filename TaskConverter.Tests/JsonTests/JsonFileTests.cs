using TaskConverter.Commons.Utils;
using TaskConverter.Plugin.GTD;
using TaskConverter.Tests.JsonTests;
using TaskConverter.Tests.Utils;

namespace TaskConverter.Tests.JsonTests;

//TODO HH: simplify
public class JsonFileTests
{
    private const string originalFilePath = "./GTD.json";
    private const string resultFilePath = "./GTD_new.json";

    [Fact]
    public void ReadAndWrite_Json_ShouldBeValid()
    {
        var task = Create.A.JsonTask().Build();
        var originalJson = Create.A.JsonData().AddTask(task).Build();
        var originalFile = new FileInfo(originalFilePath);
        var resultFile = new FileInfo(resultFilePath);

        File.WriteAllText(originalFilePath, originalJson);

        var jsonReader = new JsonConfigurationReader(originalFile);
        jsonReader.Write(resultFile);
    }

    [Fact]
    public void ReadAndWrite_Zip_ShouldBeValid()
    {
        var task = Create.A.JsonTask().Build();
        var originalJson = Create.A.JsonData().AddTask(task).Build();
        var zipFile = new FileInfo($"{originalFilePath}.zip");
        var resultFile = new FileInfo(resultFilePath);

        zipFile.WriteToZip(originalJson);

        var jsonReader = new JsonConfigurationReader(zipFile);
        jsonReader.Write(resultFile);
    }
}
