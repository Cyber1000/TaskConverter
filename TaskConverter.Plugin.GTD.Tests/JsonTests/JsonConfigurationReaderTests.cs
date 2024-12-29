using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using FluentValidation;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Tests.Utils;

namespace TaskConverter.Plugin.GTD.Tests.JsonTests;

public class JsonConfigurationReaderTests
{
    private const string originalFilePath = "./GTD.json";
    private const string resultFilePath = "./GTD_new.json";
    private readonly MockFileSystem _mockFileSystem;
    private readonly TestJsonConfigurationSerializer _jsonConfigurationSerializer;

    public JsonConfigurationReaderTests()
    {
        _mockFileSystem = new MockFileSystem();
        _jsonConfigurationSerializer = new TestJsonConfigurationSerializer();
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

        Assert.Throws<JsonException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, _jsonConfigurationSerializer));
    }

    [Fact]
    public void ReadInvalidJsonFile_ShouldThrowException()
    {
        _mockFileSystem.AddFile(originalFilePath, new MockFileData("This is not valid JSON"));

        Assert.Throws<JsonException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, _jsonConfigurationSerializer));
    }

    [Fact]
    public void ReadEmptyZipFile_ShouldThrowException()
    {
        var emptyZipContent = CreateZipContent(originalFilePath, string.Empty);
        _mockFileSystem.AddFile($"{originalFilePath}.zip", new MockFileData(emptyZipContent));

        Assert.Throws<JsonException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New($"{originalFilePath}.zip"), _mockFileSystem, _jsonConfigurationSerializer));
    }

    private JsonConfigurationReader SetupTest(string filePath)
    {
        var task = Create.A.JsonTask().Build();
        var originalJson = Create.A.JsonData().AddTask(task).Build();

        var mockFileData = filePath.EndsWith(".zip") ? new MockFileData(CreateZipContent(filePath, originalJson)) : new MockFileData(originalJson);

        _mockFileSystem.AddFile(filePath, mockFileData);
        return new JsonConfigurationReader(_mockFileSystem.FileInfo.New(filePath), _mockFileSystem, _jsonConfigurationSerializer);
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

    [Fact]
    public void ReadFolderWithNonEmptyUuid_ShouldThrowException()
    {
        var folder = Create.A.JsonFolder().WithUuid("123").Build();
        var json = Create.A.JsonData().AddFolder(folder).Build();
        _mockFileSystem.AddFile(originalFilePath, new MockFileData(json));

        var exception = Assert.Throws<ValidationException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, _jsonConfigurationSerializer));
        Assert.Contains("Only empty Uuids are accepted.", exception.Message);
    }

    [Fact]
    public void ReadFolderWithEmptyTitle_ShouldThrowException()
    {
        var folder = Create.A.JsonFolder().WithTitle("").Build();
        var json = Create.A.JsonData().AddFolder(folder).Build();
        _mockFileSystem.AddFile(originalFilePath, new MockFileData(json));

        var exception = Assert.Throws<ValidationException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, _jsonConfigurationSerializer));
        Assert.Contains("'Title' must not be empty.", exception.Message);
    }

    [Fact]
    public void ReadTagWithNonEmptyUuid_ShouldThrowException()
    {
        var tag = Create.A.JsonTag().WithUuid("123").Build();
        var json = Create.A.JsonData().AddTag(tag).Build();
        _mockFileSystem.AddFile(originalFilePath, new MockFileData(json));

        var exception = Assert.Throws<ValidationException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, _jsonConfigurationSerializer));
        Assert.Contains("Only empty Uuids are accepted", exception.Message);
    }

    [Fact]
    public void ReadTagWithEmptyTitle_ShouldThrowException()
    {
        var tag = Create.A.JsonTag().WithTitle("").Build();
        var json = Create.A.JsonData().AddTag(tag).Build();
        _mockFileSystem.AddFile(originalFilePath, new MockFileData(json));

        var exception = Assert.Throws<ValidationException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, _jsonConfigurationSerializer));
        Assert.Contains("'Title' must not be empty.", exception.Message);
    }

    [Fact]
    public void ReadTaskWithNonEmptyUuid_ShouldThrowException()
    {
        var task = Create.A.JsonTask().WithUuid("123").Build();
        var json = Create.A.JsonData().AddTask(task).Build();
        _mockFileSystem.AddFile(originalFilePath, new MockFileData(json));

        var exception = Assert.Throws<ValidationException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, _jsonConfigurationSerializer));
        Assert.Contains("Only empty Uuids are accepted", exception.Message);
    }

    [Fact]
    public void ReadTaskWithEmptyTitle_ShouldThrowException()
    {
        var task = Create.A.JsonTask().WithTitle("").Build();
        var json = Create.A.JsonData().AddTask(task).Build();
        _mockFileSystem.AddFile(originalFilePath, new MockFileData(json));

        var exception = Assert.Throws<ValidationException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, _jsonConfigurationSerializer));
        Assert.Contains("'Title' must not be empty.", exception.Message);
    }

    [Fact]
    public void ReadContextWithNonEmptyUuid_ShouldThrowException()
    {
        var context = Create.A.JsonContext().WithUuid("123").Build();
        var json = Create.A.JsonData().AddContext(context).Build();
        _mockFileSystem.AddFile(originalFilePath, new MockFileData(json));

        var exception = Assert.Throws<ValidationException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, _jsonConfigurationSerializer));
        Assert.Contains("Only empty Uuids are accepted", exception.Message);
    }

    [Fact]
    public void ReadContextWithEmptyTitle_ShouldThrowException()
    {
        var context = Create.A.JsonContext().WithTitle("").Build();
        var json = Create.A.JsonData().AddContext(context).Build();
        _mockFileSystem.AddFile(originalFilePath, new MockFileData(json));

        var exception = Assert.Throws<ValidationException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, _jsonConfigurationSerializer));
        Assert.Contains("'Title' must not be empty.", exception.Message);
    }

    [Fact]
    public void ReadNotebookWithNonEmptyUuid_ShouldThrowException()
    {
        var notebook = Create.A.JsonNotebook().WithUuid("123").Build();
        var json = Create.A.JsonData().AddNotebook(notebook).Build();
        _mockFileSystem.AddFile(originalFilePath, new MockFileData(json));

        var exception = Assert.Throws<ValidationException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, _jsonConfigurationSerializer));
        Assert.Contains("Only empty Uuids are accepted", exception.Message);
    }

    [Fact]
    public void ReadNotebookWithEmptyTitle_ShouldThrowException()
    {
        var notebook = Create.A.JsonNotebook().WithTitle("").Build();
        var json = Create.A.JsonData().AddNotebook(notebook).Build();
        _mockFileSystem.AddFile(originalFilePath, new MockFileData(json));

        var exception = Assert.Throws<ValidationException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, _jsonConfigurationSerializer));
        Assert.Contains("'Title' must not be empty.", exception.Message);
    }

    [Fact]
    public void ReadTaskWithStartDate_ShouldThrowException()
    {
        var task = Create.A.JsonTask().WithStartDate("2023-12-31 00:00").Build();
        var json = Create.A.JsonData().AddTask(task).Build();
        _mockFileSystem.AddFile(originalFilePath, new MockFileData(json));

        var exception = Assert.Throws<ValidationException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, _jsonConfigurationSerializer));
        Assert.Contains("'Start Date' not implemented.", exception.Message);
    }

    [Fact]
    public void ReadTaskWithStartTimeSet_ShouldThrowException()
    {
        var task = Create.A.JsonTask().WithStartTimeSet(true).Build();
        var json = Create.A.JsonData().AddTask(task).Build();
        _mockFileSystem.AddFile(originalFilePath, new MockFileData(json));

        var exception = Assert.Throws<ValidationException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, _jsonConfigurationSerializer));
        Assert.Contains("'Start Time Set' not implemented.", exception.Message);
    }

    [Fact]
    public void ReadTaskWithUnsupportedDueDateModifier_ShouldThrowException()
    {
        //nur 0 oder 3 erlaubt
        var task = Create.A.JsonTask().WithDueDateModifier(((int)DueDateModifier.DueAfter).ToString()).Build();
        var json = Create.A.JsonData().AddTask(task).Build();
        _mockFileSystem.AddFile(originalFilePath, new MockFileData(json));

        var exception = Assert.Throws<ValidationException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, _jsonConfigurationSerializer));
        Assert.Contains("'Due date' DueAfter not implemented.", exception.Message);
    }

    [Fact]
    public void ReadTaskWithDuration_ShouldThrowException()
    {
        var task = Create.A.JsonTask().WithDuration(60).Build();
        var json = Create.A.JsonData().AddTask(task).Build();
        _mockFileSystem.AddFile(originalFilePath, new MockFileData(json));

        var exception = Assert.Throws<ValidationException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, _jsonConfigurationSerializer));
        Assert.Contains("'Duration' not implemented.", exception.Message);
    }

    [Fact]
    public void ReadTaskWithGoal_ShouldThrowException()
    {
        var task = Create.A.JsonTask().WithGoal(1).Build();
        var json = Create.A.JsonData().AddTask(task).Build();
        _mockFileSystem.AddFile(originalFilePath, new MockFileData(json));

        var exception = Assert.Throws<ValidationException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, _jsonConfigurationSerializer));
        Assert.Contains("'Goal' not implemented.", exception.Message);
    }

    [Fact]
    public void ReadTaskWithTrashBin_ShouldThrowException()
    {
        var task = Create.A.JsonTask().WithTrashBin("trash").Build();
        var json = Create.A.JsonData().AddTask(task).Build();
        _mockFileSystem.AddFile(originalFilePath, new MockFileData(json));

        var exception = Assert.Throws<ValidationException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, _jsonConfigurationSerializer));
        Assert.Contains("'TrashBin' not implemented.", exception.Message);
    }

    [Fact]
    public void ReadTaskWithImportance_ShouldThrowException()
    {
        var task = Create.A.JsonTask().WithImportance(1).Build();
        var json = Create.A.JsonData().AddTask(task).Build();
        _mockFileSystem.AddFile(originalFilePath, new MockFileData(json));

        var exception = Assert.Throws<ValidationException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, _jsonConfigurationSerializer));
        Assert.Contains("'Importance' not implemented.", exception.Message);
    }

    [Fact]
    public void ReadTaskWithMetaInformation_ShouldThrowException()
    {
        var task = Create.A.JsonTask().WithMetaInformation("meta").Build();
        var json = Create.A.JsonData().AddTask(task).Build();
        _mockFileSystem.AddFile(originalFilePath, new MockFileData(json));

        var exception = Assert.Throws<ValidationException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, _jsonConfigurationSerializer));
        Assert.Contains("'MetaInformation' not implemented.", exception.Message);
    }

    [Fact]
    public void ReadTaskWithUnsupportedHideValue_ShouldThrowException()
    {
        var task = Create.A.JsonTask().WithHide(((int)Hide.GivenDate).ToString()).Build();
        var json = Create.A.JsonData().AddTask(task).Build();
        _mockFileSystem.AddFile(originalFilePath, new MockFileData(json));

        var exception = Assert.Throws<ValidationException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, _jsonConfigurationSerializer));
        Assert.Contains("'Hide' not implemented with value GivenDate.", exception.Message);
    }

    [Fact]
    public void ReadTaskNote_ShouldThrowException()
    {
        var taskNote = Create.A.JsonTaskNote().Build();
        var json = Create.A.JsonData().AddTaskNote(taskNote).Build();
        _mockFileSystem.AddFile(originalFilePath, new MockFileData(json));

        var exception = Assert.Throws<ValidationException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, _jsonConfigurationSerializer));
        Assert.Contains("'TaskNote' not implemented.", exception.Message);
    }

    [Fact]
    public void ReadWithoutPreferences_ShouldThrowException()
    {
        var task = Create.A.JsonTask().Build();
        var json = Create.A.JsonData().AddTask(task).WithoutPreferences().Build();
        _mockFileSystem.AddFile(originalFilePath, new MockFileData(json));

        var exception = Assert.Throws<ValidationException>(() => new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, _jsonConfigurationSerializer));
        Assert.Contains("'Preferences' must not be empty.", exception.Message);
    }

    [Fact]
    public void Validate_WithValidJsonAndXml_ShouldReturnNoError()
    {
        var task = Create.A.JsonTask().Build();
        var json = Create.A.JsonData().AddTask(task).Build();
        _mockFileSystem.AddFile(originalFilePath, new MockFileData(json));
        var reader = new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, _jsonConfigurationSerializer);

        var (isError, validationError) = reader.ValidateRoundtrip();

        Assert.False(isError);
        Assert.Empty(validationError);
    }

    [Fact]
    public void Validate_WithModifiedJson_ShouldReturnError()
    {
        var testJsonConfigurationSerializer = new TestJsonConfigurationSerializer();
        var task = Create.A.JsonTask().Build();
        var originalJson = Create.A.JsonData().AddTask(task).Build();
        var modifiedJson = originalJson.Replace("\"TITLE\": \"Test\"", "\"TITLE\": \"Modified Test\"");
        testJsonConfigurationSerializer.OverrideSerializerString(modifiedJson);

        _mockFileSystem.AddFile(originalFilePath, new MockFileData(originalJson));
        var reader = new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, testJsonConfigurationSerializer);
        var (isError, validationError) = reader.ValidateRoundtrip();

        Assert.True(isError);
        Assert.Contains("\"original\": \"Test\"", validationError);
        Assert.Contains("\"new\": \"Modified Test\"", validationError);
        testJsonConfigurationSerializer.ResetSerializerString();
    }

    [Fact]
    public void Validate_WithModifiedXml_ShouldReturnError()
    {
        var testJsonConfigurationSerializer = new TestJsonConfigurationSerializer();
        var preferences = "<preferences><setting>modified</setting></preferences>";
        var originalJson = Create.A.JsonData().WithPreferences(preferences).Build();
        var modifiedJson = originalJson.Replace("preferences>", "special>");
        testJsonConfigurationSerializer.OverrideSerializerString(modifiedJson);

        _mockFileSystem.AddFile(originalFilePath, new MockFileData(originalJson));
        var reader = new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, testJsonConfigurationSerializer);
        var (isError, validationError) = reader.ValidateRoundtrip();

        Assert.True(isError);
        Assert.Contains("<xd:change match=\"1\" name=\"special\" /></xd:xmldiff>", validationError);
        testJsonConfigurationSerializer.ResetSerializerString();
    }

    [Fact]
    public void Validate_WithWrongXmlHeader_ShouldThrowException()
    {
        var testJsonConfigurationSerializer = new TestJsonConfigurationSerializer();
        var originalJson = Create.A.JsonData().Build();
        var modifiedJson = originalJson.Replace("com.dg.gtd.android.lite_preferences", "com.xyz.gtd.android.lite_preferences");
        testJsonConfigurationSerializer.OverrideSerializerString(modifiedJson);

        _mockFileSystem.AddFile(originalFilePath, new MockFileData(originalJson));
        var reader = new JsonConfigurationReader(_mockFileSystem.FileInfo.New(originalFilePath), _mockFileSystem, testJsonConfigurationSerializer);
        var ex = Record.Exception(() => reader.ValidateRoundtrip());

        Assert.NotNull(ex);
        Assert.Equal("XML preferences section not found in JSON input.", ex.Message);
        testJsonConfigurationSerializer.ResetSerializerString();
    }
}
