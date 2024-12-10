using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using TaskConverter.Plugin.GTD.Tests.Utils;

namespace TaskConverter.Plugin.GTD.Tests.JsonTests;

public class JsonFileTests
{
    private const string originalFilePath = "./GTD.json";
    private const string resultFilePath = "./GTD_new.json";
    private readonly MockFileSystem _mockFileSystem;

    public JsonFileTests()
    {
        _mockFileSystem = new MockFileSystem();
    }

    [Fact]
    public void ReadAndWriteJsonFile_ShouldPreserveContent()
    {
        var jsonReader = SetupTest(originalFilePath);
        jsonReader.Write(_mockFileSystem.FileInfo.New(resultFilePath));

        var writtenContent = _mockFileSystem.File.ReadAllText(resultFilePath);
        
        Assert.NotEmpty(writtenContent);
        Assert.Contains("\"version\":", writtenContent);
        Assert.Contains("\"TASK\":", writtenContent);
    }

    [Fact]
    public void ReadAndWriteZipFile_ShouldPreserveContent()
    {
        var jsonReader = SetupTest($"{originalFilePath}.zip");
        jsonReader.Write(_mockFileSystem.FileInfo.New(resultFilePath));

        var writtenContent = _mockFileSystem.File.ReadAllText(resultFilePath);

        Assert.NotEmpty(writtenContent);
        Assert.Contains("\"version\":", writtenContent);
        Assert.Contains("\"TASK\":", writtenContent);
    }

    [Fact]
    public void ReadEmptyJsonFile_ShouldThrowException()
    {
        _mockFileSystem.AddFile(originalFilePath, new MockFileData(string.Empty));

        Assert.Throws<JsonException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem));
    }

    [Fact]
    public void ReadInvalidJsonFile_ShouldThrowException()
    {
        _mockFileSystem.AddFile(originalFilePath, new MockFileData("This is not valid JSON"));

        Assert.Throws<JsonException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem));
    }

    [Fact]
    public void ReadEmptyZipFile_ShouldThrowException()
    {
        var emptyZipContent = CreateZipContent(originalFilePath, string.Empty);
        _mockFileSystem.AddFile($"{originalFilePath}.zip", new MockFileData(emptyZipContent));

        Assert.Throws<JsonException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New($"{originalFilePath}.zip"), _mockFileSystem));
    }

    private JsonConfigurationReader SetupTest(string filePath)
    {
        var task = Create.A.JsonTask().Build();
        var originalJson = Create.A.JsonData().AddTask(task).Build();

        var mockFileData = filePath.EndsWith(".zip") ? new MockFileData(CreateZipContent(filePath, originalJson)) : new MockFileData(originalJson);

        _mockFileSystem.AddFile(filePath, mockFileData);
        return new JsonConfigurationReader(_mockFileSystem.FileInfo.New(filePath), _mockFileSystem);
    }

    private static byte[] CreateZipContent(string filePath, string content)
    {
        using var zipStream = new MemoryStream();
        using (var archive = new System.IO.Compression.ZipArchive(zipStream, System.IO.Compression.ZipArchiveMode.Create, true))
        {
            var entry = archive.CreateEntry(Path.GetFileNameWithoutExtension(filePath));
            using var entryStream = entry.Open();
            using var writer = new StreamWriter(entryStream);
            writer.Write(content);
        }
        return zipStream.ToArray();
    }
}
