using System.IO.Abstractions.TestingHelpers;
using TaskConverter.Plugin.GTD.Tests.Utils;

namespace TaskConverter.Plugin.GTD.Tests.JsonTests;

public class JsonConfigurationWriterTests : JsonConfigurationBaseTests
{
    private const string originalFilePath = "./GTD.json";
    private const string resultFilePath = "./GTD_new.json";
    private readonly MockFileSystem _mockFileSystem;
    private readonly TestJsonConfigurationSerializer _jsonConfigurationSerializer;

    public JsonConfigurationWriterTests()
    {
        _mockFileSystem = new MockFileSystem();
        _jsonConfigurationSerializer = new TestJsonConfigurationSerializer();
    }

    [Fact]
    public void ReadAndWriteJsonFile_ShouldPreserveContent()
    {
        var jsonReader = SetupTest(originalFilePath);
        var model = jsonReader.Read(originalFilePath);
        var jsonWriter = new JsonConfigurationWriter(_mockFileSystem, _jsonConfigurationSerializer);
        jsonWriter.Write(resultFilePath, model);

        var writtenContent = _mockFileSystem.File.ReadAllText(resultFilePath);

        Assert.NotEmpty(writtenContent);
        Assert.Contains("\"version\":", writtenContent);
        Assert.Contains("\"TASK\":", writtenContent);
    }

    [Fact]
    public void ReadAndWriteZipFile_ShouldPreserveContent()
    {
        var originalZipFilePath = $"{originalFilePath}.zip";
        var jsonReader = SetupTest(originalZipFilePath);
        var model = jsonReader.Read(originalZipFilePath);
        var jsonWriter = new JsonConfigurationWriter(_mockFileSystem, _jsonConfigurationSerializer);
        jsonWriter.Write(resultFilePath, model);

        var writtenContent = _mockFileSystem.File.ReadAllText(resultFilePath);

        Assert.NotEmpty(writtenContent);
        Assert.Contains("\"version\":", writtenContent);
        Assert.Contains("\"TASK\":", writtenContent);
    }

    private JsonConfigurationReader SetupTest(string filePath)
    {
        var task = Create.A.JsonTask().Build();
        var originalJson = Create.A.JsonData().AddTask(task).Build();

        var mockFileData = filePath.EndsWith(".zip") ? new MockFileData(CreateZipContent(filePath, originalJson)) : new MockFileData(originalJson);

        _mockFileSystem.AddFile(filePath, mockFileData);
        return new JsonConfigurationReader(_mockFileSystem, _jsonConfigurationSerializer);
    }
}