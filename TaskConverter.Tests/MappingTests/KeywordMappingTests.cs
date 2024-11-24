using NodaTime;
using TaskConverter.Model.Mapper;
using TaskConverter.Model.Model;
using TaskConverter.Plugin.GTD.Mapper;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Tests.TestData;

namespace TaskConverter.Tests.MappingTests;

//TODO HH: simplify (use asserts between objects, maybe use a general model for mapping)
public class KeywordMappingTests(IConverter testConverter, IClock clock, IConverterDateTimeZoneProvider converterDateTimeZoneProvider) : BaseMappingTests(testConverter, clock, converterDateTimeZoneProvider)
{

    [Fact]
    public void Map_Folder()
    {
        var gtdDataModel = CreateGTDDataModelWithFolder();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var gtdFolderModel = gtdDataModel.Folder![0];
        var taskAppFolderModel = taskAppDataModel!.KeyWords!.First(t => t.KeyWordType == GTDKeyWordEnum.Folder);
        var gtdRemappedFolderModel = gtdDataMappedRemappedModel?.Folder?[0]!;

        AssertMappedModelEquivalence(gtdFolderModel, taskAppFolderModel, gtdRemappedFolderModel);
    }

    [Fact]
    public void Map_Context()
    {
        var gtdDataModel = CreateGTDDataModelWithContext();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var gtdContextModel = gtdDataModel.Context![0];
        var taskAppContextModel = taskAppDataModel!.KeyWords!.First(t => t.KeyWordType == GTDKeyWordEnum.Context);
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
        var taskAppTagModel = taskAppDataModel!.KeyWords!.First(t => t.KeyWordType == KeyWordEnum.Tag);
        var gtdRemappedTagModel = gtdDataMappedRemappedModel?.Tag?[0]!;

        AssertMappedModelEquivalence(gtdTagModel, taskAppTagModel, gtdRemappedTagModel);
    }

    private static GTDDataModel CreateGTDDataModelWithFolder()
    {
        return Create.A.GTDDataModel().AddFolder(TestConstants.DefaultFolderId).Build();
    }

    private static GTDDataModel CreateGTDDataModelWithContext()
    {
        return Create.A.GTDDataModel().AddContext(TestConstants.DefaultContextId).Build();
    }

    private static GTDDataModel CreateGTDDataModelWithTag()
    {
        return Create.A.GTDDataModel().AddTag(TestConstants.DefaultTagId).Build();
    }
}
