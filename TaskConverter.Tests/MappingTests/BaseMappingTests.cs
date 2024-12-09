using System.Drawing;
using NodaTime;
using TaskConverter.Model.Mapper;
using TaskConverter.Model.Model;
using TaskConverter.Plugin.GTD.Mapper;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Tests.Utils;

namespace TaskConverter.Tests.MappingTests;

public abstract class BaseMappingTests(IConverter testConverter, IClock clock, IConverterDateTimeZoneProvider converterDateTimeZoneProvider)
{
    protected readonly IConverter TestConverter = testConverter;
    protected readonly IClock clock = clock;
    protected readonly IConverterDateTimeZoneProvider converterDateTimeZoneProvider = converterDateTimeZoneProvider;
    protected DateTimeZone CurrentDateTimeZone => TestConverter.DateTimeZoneProvider.CurrentDateTimeZone;

    public static class TestConstants
    {
        public const int DefaultFolderId = 1;
        public const int DefaultContextId = 2;
        public const int DefaultTagId = 3;
        public const int DefaultTaskId = 4;
        public const int DefaultTaskNoteId = 5;
        public const int DefaultNotebookId = 6;
        public const long DefaultDueDateMilliseconds = 1608541200000;
    }

    protected (TaskAppDataModel? model, GTDDataModel? fromModel) GetMappedInfo(GTDDataModel gtdDataModel)
    {
        if (gtdDataModel == null)
            return (null, null);
        var taskAppDataModel = TestConverter.MapToModel(gtdDataModel);
        var gtdDataMappedRemappedModel = TestConverter.MapFromModel(taskAppDataModel);

        return (taskAppDataModel, gtdDataMappedRemappedModel);
    }

    protected void AssertCommonProperties<T>(T gtdModel, ExtendedModel taskAppModel)
        where T : GTDExtendedModel
    {
        Assert.Equal(gtdModel.Id.ToString(), taskAppModel.Id);
        Assert.Equal(gtdModel.Created, taskAppModel.Created.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(gtdModel.Modified, taskAppModel.Modified.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(gtdModel.Title, taskAppModel.Title);
        Assert.Equal(Color.FromArgb(gtdModel.Color), taskAppModel.Color);
        Assert.Equal(gtdModel.Visible, taskAppModel.Visible);
    }

    protected void AssertMappedModelEquivalence<T>(T originalModel, ExtendedModel taskAppModel, T remappedModel)
        where T : class
    {
        Assert.NotNull(taskAppModel);
        Assert.NotNull(remappedModel);

        if (originalModel is GTDExtendedModel gtdExtendedModel)
        {
            AssertCommonProperties(gtdExtendedModel, taskAppModel);
        }

        Assert.Equivalent(originalModel, remappedModel);
    }
}
