using NodaTime;
using TaskConverter.Commons;
using TaskConverter.Commons.Utils;
using TaskConverter.Plugin.GTD.Conversion;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Tests.Utils;
using TaskConverter.Plugin.GTD.TodoModel;

namespace TaskConverter.Plugin.GTD.Tests.MappingTests;

public class NotebookMappingTests(IConversionService<GTDDataModel> testConverter, IClock clock, ISettingsProvider settingsProvider, IKeyWordMapperService keyWordMapperService)
    : BaseMappingTests(testConverter, clock, settingsProvider)
{
    [Fact]
    public void Map_Notebook()
    {
        var gtdDataModel = CreateGTDDataModelWithNotebook();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var gtdNotebookModel = gtdDataModel.Notebook![0];
        var taskAppNotebookModel = taskAppDataModel?.Journals?[0]!;
        var gtdRemappedNotebookModel = gtdDataMappedRemappedModel?.Notebook?[0]!;

        AssertMappedModelEquivalence(gtdNotebookModel, taskAppNotebookModel, gtdRemappedNotebookModel);
        Assert.Equal(gtdNotebookModel.Note, taskAppNotebookModel.Description?.GetStringArray());
        var taskAppFolderModel = keyWordMapperService.GetKeyWordMetaDataIntermediateFormatDictionary(taskAppDataModel!, CurrentDateTimeZone).Values.First(t => t.KeyWordType == KeyWordType.Folder);
        Assert.Equal(gtdNotebookModel.FolderId, taskAppFolderModel.Id);
    }

    private static GTDDataModel CreateGTDDataModelWithNotebook()
    {
        return Create.A.GTDDataModel().AddFolder(TestConstants.DefaultFolderId).AddNotebook(TestConstants.DefaultNotebookId, TestConstants.DefaultFolderId).Build();
    }
}
