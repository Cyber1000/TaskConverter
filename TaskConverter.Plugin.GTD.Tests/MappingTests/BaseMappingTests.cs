using System.Drawing;
using Ical.Net;
using Ical.Net.CalendarComponents;
using NodaTime;
using TaskConverter.Plugin.GTD.Mapper;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Tests.Utils;
using TaskConverter.Plugin.GTD.TodoModel;
using TaskConverter.Plugin.GTD.Utils;

namespace TaskConverter.Plugin.GTD.Tests.MappingTests;

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

    protected (Calendar? model, GTDDataModel? fromModel) GetMappedInfo(GTDDataModel gtdDataModel)
    {
        if (gtdDataModel == null)
            return (null, null);
        var taskAppDataModel = TestConverter.MapToModel(gtdDataModel);
        var gtdDataMappedRemappedModel = TestConverter.MapFromModel(taskAppDataModel);

        return (taskAppDataModel, gtdDataMappedRemappedModel);
    }

    protected void AssertCommonProperties<T>(T gtdModel, RecurringComponent recurringComponent)
        where T : GTDExtendedModel
    {
        Assert.Equal(gtdModel.Id.ToString(), recurringComponent.Uid);
        Assert.Equal(gtdModel.Created, recurringComponent.Created.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(gtdModel.Modified, recurringComponent.LastModified.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(gtdModel.Title, recurringComponent.Summary);
        Assert.Equal(Color.FromArgb(gtdModel.Color), recurringComponent.Properties.Get<Color?>(nameof(GTDExtendedModel.Color)));
        Assert.Equal(gtdModel.Visible, bool.Parse(recurringComponent.Properties.Get<string>(nameof(GTDExtendedModel.Visible))));
    }

    protected void AssertCommonProperties<T>(T gtdModel, KeyWordMetaData keyWordMetaData)
        where T : GTDExtendedModel
    {
        Assert.Equal(gtdModel.Id, keyWordMetaData.Id);
        Assert.Equal(gtdModel.Created, keyWordMetaData.Created.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(gtdModel.Modified, keyWordMetaData.Modified.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(gtdModel.Title, keyWordMetaData.Name);
        Assert.Equal(GetKeyWordType(gtdModel), keyWordMetaData.KeyWordType);
        Assert.Equal(Color.FromArgb(gtdModel.Color), keyWordMetaData.Color);

        KeyWordType GetKeyWordType(T gtdModel)
        {
            return gtdModel switch
            {
                GTDContextModel => KeyWordType.Context,
                GTDFolderModel => KeyWordType.Folder,
                GTDTagModel => KeyWordType.Tag,
                _ => throw new NotImplementedException($"KeyWordType for {gtdModel.GetType().Name} not implemented."),
            };
        }
    }

    protected void AssertMappedModelEquivalence<T>(T originalModel, RecurringComponent recurringComponent, T remappedModel)
        where T : class
    {
        Assert.NotNull(recurringComponent);
        Assert.NotNull(remappedModel);

        if (originalModel is GTDExtendedModel gtdExtendedModel)
        {
            AssertCommonProperties(gtdExtendedModel, recurringComponent);
        }

        Assert.Equivalent(originalModel, remappedModel);
    }

    protected void AssertMappedModelEquivalence<T>(T originalModel, KeyWordMetaData keyWordMetaData, T remappedModel)
        where T : class
    {
        Assert.NotNull(remappedModel);

        if (originalModel is GTDExtendedModel gtdExtendedModel)
        {
            AssertCommonProperties(gtdExtendedModel, keyWordMetaData);
        }

        Assert.Equivalent(originalModel, remappedModel);
    }
}
