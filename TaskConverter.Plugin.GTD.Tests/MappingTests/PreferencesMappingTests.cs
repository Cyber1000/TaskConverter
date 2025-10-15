using NodaTime;
using TaskConverter.Plugin.Base;
using TaskConverter.Plugin.GTD.Conversion;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Tests.Utils;

namespace TaskConverter.Plugin.GTD.Tests.MappingTests;

public class PreferencesMappingTests(IConversionService<GTDDataModel> testConverter, IClock clock) : BaseMappingTests(testConverter, clock)
{
    [Fact]
    public void SettingsToIntermediateFormat_WithPreferencePath_ShouldWriteFile()
    {
        var fileSystem = ((ConversionService)TestConverter).FileSystem;
        ((TestSettingsProvider)TestConverter.SettingsProvider).PreferenceFilePath = "preferences.xml";

        var gtdDataModel = Create.A.GTDDataModel().AddPreferences().Build();
        TestConverter.MapToIntermediateFormat(gtdDataModel);

        Assert.Single(fileSystem.Directory.EnumerateFileSystemEntries("/", "*.*", SearchOption.AllDirectories));
        Assert.True(fileSystem.File.Exists("preferences.xml"));
    }

    [Fact]
    public void SettingsToIntermediateFormat_WithoutPreferencePath_ShouldNotWriteFile()
    {
        var fileSystem = ((ConversionService)TestConverter).FileSystem;
        ((TestSettingsProvider)TestConverter.SettingsProvider).PreferenceFilePath = "";

        var gtdDataModel = Create.A.GTDDataModel().AddPreferences().Build();
        TestConverter.MapToIntermediateFormat(gtdDataModel);

        Assert.Empty(fileSystem.Directory.EnumerateFileSystemEntries("/", "*.*", SearchOption.AllDirectories));
    }

    [Fact]
    public void SettingsFromIntermediateFormat_WithPreferencePath_ShouldReadFile()
    {
        ((TestSettingsProvider)TestConverter.SettingsProvider).PreferenceFilePath = "preferences.xml";

        var gtdDataModel = Create.A.GTDDataModel().AddPreferences().Build();
        var taskAppDataModel = TestConverter.MapToIntermediateFormat(gtdDataModel);
        var gtdDataMappedRemappedModel = TestConverter.MapFromIntermediateFormat(taskAppDataModel);

        Assert.NotNull(gtdDataMappedRemappedModel.Preferences?.FirstOrDefault()?.XmlConfig);
        Assert.Equivalent(gtdDataMappedRemappedModel.Preferences, gtdDataModel.Preferences);
    }

    [Fact]
    public void SettingsFromIntermediateFormat_WithPreferencePathButMissingFile_ShouldNotReadFile()
    {
        ((TestSettingsProvider)TestConverter.SettingsProvider).PreferenceFilePath = "";
        var gtdDataModel = Create.A.GTDDataModel().AddPreferences().Build();
        var taskAppDataModel = TestConverter.MapToIntermediateFormat(gtdDataModel);

        ((TestSettingsProvider)TestConverter.SettingsProvider).PreferenceFilePath = "preferences.xml";
        var gtdDataMappedRemappedModel = TestConverter.MapFromIntermediateFormat(taskAppDataModel);

        Assert.Null(gtdDataMappedRemappedModel.Preferences?.FirstOrDefault()?.XmlConfig);
    }

    [Fact]
    public void SettingsFromIntermediateFormat_WithoutPreferencePathButExistingFile_ShouldNotReadFile()
    {
        ((TestSettingsProvider)TestConverter.SettingsProvider).PreferenceFilePath = "preferences.xml";
        var gtdDataModel = Create.A.GTDDataModel().AddPreferences().Build();
        var taskAppDataModel = TestConverter.MapToIntermediateFormat(gtdDataModel);

        ((TestSettingsProvider)TestConverter.SettingsProvider).PreferenceFilePath = "";
        var gtdDataMappedRemappedModel = TestConverter.MapFromIntermediateFormat(taskAppDataModel);

        Assert.Null(gtdDataMappedRemappedModel.Preferences?.FirstOrDefault()?.XmlConfig);
    }
}
