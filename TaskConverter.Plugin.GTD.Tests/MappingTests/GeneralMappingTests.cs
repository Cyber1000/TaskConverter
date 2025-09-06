using NodaTime;
using TaskConverter.Plugin.GTD.Mapper;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Tests.Utils;

namespace TaskConverter.Plugin.GTD.Tests.MappingTests;

public class GeneralMappingTests(IConversionService<GTDDataModel> testConverter, IClock clock, IConverterDateTimeZoneProvider converterDateTimeZoneProvider)
    : BaseMappingTests(testConverter, clock, converterDateTimeZoneProvider)
{
    [Fact]
    public void Automapper_CheckConfig()
    {
        var gtdMapper = new ConversionService(clock, converterDateTimeZoneProvider);

        gtdMapper.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_Version()
    {
        var gtdDataModel = CreateGTDDataModel();

        var (_, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var gtdVersionModel = gtdDataModel.Version;
        var gtdRemappedVersionModel = gtdDataMappedRemappedModel?.Version;

        Assert.Equal(gtdVersionModel, gtdRemappedVersionModel);
    }

    private static GTDDataModel CreateGTDDataModel()
    {
        return Create.A.GTDDataModel().Build();
    }
}
