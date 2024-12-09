using NodaTime;
using TaskConverter.Model.Mapper;
using TaskConverter.Plugin.GTD.Mapper;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Tests.Utils;

namespace TaskConverter.Tests.MappingTests;

//TODO HH: simplify (use asserts between objects, maybe use a general model for mapping)
public class GeneralMappingTests(IConverter testConverter, IClock clock, IConverterDateTimeZoneProvider converterDateTimeZoneProvider) : BaseMappingTests(testConverter, clock, converterDateTimeZoneProvider)
{
    [Fact]
    public void Automapper_CheckConfig()
    {
        var gtdMapper = new GTDMapper(clock, converterDateTimeZoneProvider);

        gtdMapper.Mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_Version()
    {
        var gtdDataModel = CreateGTDDataModel();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var gtdVersionModel = gtdDataModel.Version;
        var taskAppVersionModel = taskAppDataModel?.Version;
        var gtdRemappedVersionModel = gtdDataMappedRemappedModel?.Version;

        Assert.Equal(gtdVersionModel, taskAppVersionModel);
        Assert.Equal(gtdVersionModel, gtdRemappedVersionModel);
    }

    private static GTDDataModel CreateGTDDataModel()
    {
        return Create.A.GTDDataModel().Build();
    }
}
