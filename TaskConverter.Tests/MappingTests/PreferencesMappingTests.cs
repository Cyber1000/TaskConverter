using NodaTime;
using TaskConverter.Model.Mapper;
using TaskConverter.Plugin.GTD.Mapper;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Tests.MappingTests;

//TODO HH: simplify (use asserts between objects, maybe use a general model for mapping)
public class PreferencesMappingTests(IConverter testConverter, IClock clock, IConverterDateTimeZoneProvider converterDateTimeZoneProvider) : BaseMappingTests(testConverter, clock, converterDateTimeZoneProvider)
{
    [Fact]
    public void Map_Preferences()
    {
        var gtdDataModel = CreateGTDDataModelWithPreferences();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var gtdPreferenceModel = gtdDataModel.Preferences![0].XmlConfig;
        var taskAppPreferenceModel = taskAppDataModel?.Config;
        var gtdRemappedPreferenceModel = gtdDataMappedRemappedModel?.Preferences?[0]?.XmlConfig;

        Assert.Equal(taskAppPreferenceModel, gtdPreferenceModel);
        Assert.Equal(gtdRemappedPreferenceModel, gtdPreferenceModel);
    }

    private static GTDDataModel CreateGTDDataModelWithPreferences()
    {
        return Create.A.GTDDataModel().AddPreferences().Build();
    }
}
