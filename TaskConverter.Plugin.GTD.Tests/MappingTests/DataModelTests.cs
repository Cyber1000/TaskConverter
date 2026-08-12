using NodaTime;
using TaskConverter.Plugin.Base;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Tests.Utils;
using TaskConverter.Plugin.GTD.Validators;

namespace TaskConverter.Plugin.GTD.Tests.MappingTests;

public class DataModelTests(IConversionService<GTDDataModel> testConverter, IClock clock) : BaseMappingTests(testConverter, clock)
{
    [Fact]
    public void GetAllEntries_ReturnsEveryEntityType()
    {
        var gtdDataModel = Create
            .A.GTDDataModel()
            .AddFolder(TestConstants.DefaultFolderId)
            .AddContext(TestConstants.DefaultContextId)
            .AddTag(TestConstants.DefaultTagId)
            .AddNotebook(TestConstants.DefaultNotebookId, TestConstants.DefaultFolderId)
            .AddTaskList(() => [Create.A.GTDTaskModel(TestConstants.DefaultTaskId).Build()])
            .Build();

        var entries = gtdDataModel.GetAllEntries;

        Assert.Equal(5, entries.Count);
        Assert.Single(entries.OfType<GTDFolderModel>());
        Assert.Single(entries.OfType<GTDContextModel>());
        Assert.Single(entries.OfType<GTDTagModel>());
        Assert.Single(entries.OfType<GTDNotebookModel>());
        Assert.Single(entries.OfType<GTDTaskModel>());
    }

    /// <summary>
    /// The task rules only take effect if the task actually reaches the validator. A real backup
    /// always contains folders, so validating with a folder present is the case that matters.
    /// </summary>
    [Fact]
    public void Validate_RejectsUnsupportedTaskFields_WhenFolderIsPresent()
    {
        var gtdDataModel = Create
            .A.GTDDataModel()
            .AddPreferences()
            .AddFolder(TestConstants.DefaultFolderId)
            .AddTaskList(() => [Create.A.GTDTaskModel(TestConstants.DefaultTaskId).Build()])
            .Build();
        var task = gtdDataModel.Task!.Single();
        task.Goal = 42;
        task.Duration = 90;
        task.Importance = 7;
        task.MetaInformation = "whatever";
        task.Type = TaskType.Note;

        var result = new GTDDataModelValidator().Validate(gtdDataModel);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Goal"));
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Duration"));
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Importance"));
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("MetaInformation"));
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("TaskType"));
    }

    [Fact]
    public void MapNotebookWithoutFolder_DoesNotThrow()
    {
        var gtdDataModel = Create.A.GTDDataModel().AddNotebook(TestConstants.DefaultNotebookId, folderId: 0).Build();

        var (_, remapped) = GetMappedInfo(gtdDataModel);

        var notebook = Assert.Single(remapped!.Notebook!);
        Assert.Equal(0, notebook.FolderId);
    }
}
