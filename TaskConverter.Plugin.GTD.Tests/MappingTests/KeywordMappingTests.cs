using NodaTime;
using TaskConverter.Commons;
using TaskConverter.Plugin.GTD.Conversion;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Tests.Utils;
using TaskConverter.Plugin.GTD.TodoModel;

namespace TaskConverter.Plugin.GTD.Tests.MappingTests;

public class KeywordMappingTests(IConversionService<GTDDataModel> testConverter, IClock clock, IConverterDateTimeZoneProvider converterDateTimeZoneProvider, IKeyWordMapperService keyWordMapperService)
    : BaseMappingTests(testConverter, clock, converterDateTimeZoneProvider)
{
    [Fact]
    public void Map_Folder()
    {
        var gtdDataModel = CreateGTDDataModelWithFolder();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var gtdFolderModel = gtdDataModel.Folder![0];
        var taskAppFolderModel = keyWordMapperService.GetKeyWordMetaDataIntermediateFormatDictionary(taskAppDataModel!, CurrentDateTimeZone).Values
            .First(t => t.KeyWordType == KeyWordType.Folder);
        var gtdRemappedFolderModel = gtdDataMappedRemappedModel?.Folder?[0]!;

        AssertMappedModelEquivalence(gtdFolderModel, taskAppFolderModel, gtdRemappedFolderModel);
    }

    [Fact]
    public void Map_Context()
    {
        var gtdDataModel = CreateGTDDataModelWithContext();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var gtdContextModel = gtdDataModel.Context![0];
        var taskAppContextModel = keyWordMapperService.GetKeyWordMetaDataIntermediateFormatDictionary(taskAppDataModel!, CurrentDateTimeZone).Values
            .First(t => t.KeyWordType == KeyWordType.Context);
        var gtdRemappedContextModel = gtdDataMappedRemappedModel?.Context?[0]!;

        AssertCommonProperties(gtdContextModel, taskAppContextModel);
        AssertMappedModelEquivalence(gtdContextModel, taskAppContextModel, gtdRemappedContextModel);
    }

    [Fact]
    public void Map_Tag()
    {
        var gtdDataModel = CreateGTDDataModelWithTag();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var gtdTagModel = gtdDataModel.Tag![0];
        var taskAppTagModel = keyWordMapperService.GetKeyWordMetaDataIntermediateFormatDictionary(taskAppDataModel!, CurrentDateTimeZone).Values.First(t => t.KeyWordType == KeyWordType.Tag);
        var gtdRemappedTagModel = gtdDataMappedRemappedModel?.Tag?[0]!;

        AssertMappedModelEquivalence(gtdTagModel, taskAppTagModel, gtdRemappedTagModel);
    }

    private static GTDDataModel CreateGTDDataModelWithFolder()
    {
        var taskListBuilder = new List<GTDTaskModelBuilder> { Create.A.GTDTaskModel(TestConstants.DefaultTaskId).WithFolder(TestConstants.DefaultFolderId) };
        return Create.A.GTDDataModel().AddTaskList(() => taskListBuilder.Select(t => t.Build()).ToList()).AddFolder(TestConstants.DefaultFolderId).Build();
    }

    private static GTDDataModel CreateGTDDataModelWithContext()
    {
        var taskListBuilder = new List<GTDTaskModelBuilder> { Create.A.GTDTaskModel(TestConstants.DefaultTaskId).WithContext(TestConstants.DefaultContextId) };
        return Create.A.GTDDataModel().AddTaskList(() => taskListBuilder.Select(t => t.Build()).ToList()).AddContext(TestConstants.DefaultContextId).Build();
    }

    private static GTDDataModel CreateGTDDataModelWithTag()
    {
        var taskListBuilder = new List<GTDTaskModelBuilder> { Create.A.GTDTaskModel(TestConstants.DefaultTaskId).WithTags([TestConstants.DefaultTagId]) };
        return Create.A.GTDDataModel().AddTaskList(() => taskListBuilder.Select(t => t.Build()).ToList()).AddTag(TestConstants.DefaultTagId).Build();
    }
}
