using NodaTime;
using TaskConverter.Commons;
using TaskConverter.Plugin.GTD.Conversion;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Tests.Utils;

namespace TaskConverter.Plugin.GTD.Tests.MappingTests;

public class GeneralMappingTests(IConversionService<GTDDataModel> testConverter, IClock clock, ISettingsProvider settingsProvider) : BaseMappingTests(testConverter, clock, settingsProvider)
{
    [Fact]
    public void Automapper_CheckConfig()
    {
        ((ConversionService)TestConverter).AssertConfigurationIsValid();
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
