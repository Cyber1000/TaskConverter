using NodaTime;
using TaskConverter.Model.Mapper;
using TaskConverter.Plugin.GTD.Mapper;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Tests.Utils;

namespace TaskConverter.Tests.MappingTests;

public class NotebookMappingTests(IConverter testConverter, IClock clock, IConverterDateTimeZoneProvider converterDateTimeZoneProvider) : BaseMappingTests(testConverter, clock, converterDateTimeZoneProvider)
{

    [Fact]
    public void Map_Notebook()
    {
        var gtdDataModel = CreateGTDDataModelWithNotebook();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var gtdNotebookModel = gtdDataModel.Notebook![0];
        var taskAppNotebookModel = taskAppDataModel?.Notebooks?[0]!;
        var gtdRemappedNotebookModel = gtdDataMappedRemappedModel?.Notebook?[0]!;

        AssertMappedModelEquivalence(gtdNotebookModel, taskAppNotebookModel, gtdRemappedNotebookModel);
        Assert.Equal(gtdNotebookModel.Note, taskAppNotebookModel.Note);
        Assert.Equal(gtdNotebookModel.FolderId.ToString(), taskAppNotebookModel.Keyword?.Id);
    }

    private static GTDDataModel CreateGTDDataModelWithNotebook()
    {
        return Create.A.GTDDataModel().AddFolder(TestConstants.DefaultFolderId).AddNotebook(TestConstants.DefaultNotebookId, TestConstants.DefaultFolderId).Build();
    }
}
