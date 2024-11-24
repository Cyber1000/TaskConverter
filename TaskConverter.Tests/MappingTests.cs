using System.Drawing;
using NodaTime;
using TaskConverter.Model.Mapper;
using TaskConverter.Model.Model;
using TaskConverter.Plugin.GTD.Mapper;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Tests.Extensions;
using TaskConverter.Tests.TestData;

namespace TaskConverter.Tests;

//TODO HH: simplify (use asserts between objects, maybe use a general model for mapping)
public class MappingTests(IConverter TestConverter, IClock clock, IConverterDateTimeZoneProvider converterDateTimeZoneProvider)
{
    private DateTimeZone CurrentDateTimeZone => TestConverter.DateTimeZoneProvider.CurrentDateTimeZone;

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

    public enum ReminderTestCase
    {
        NoReminder,
        ImmediateReminder,
        ShortReminder,
        MediumReminder,
        LongReminder,
        AbsoluteReminder,
        SlightlyOverLongReminder,
        SlightlyUnderLongReminder,
        SlightlyOverMediumReminder,
    }

    public enum RepeatTestCase
    {
        EveryDay,
        EveryTwoWeeks,
        EveryThreeMonths,
        EveryFourYears,
        EveryDayLowerCase,
        NoRepeat,
    }

    public enum HideTestCase
    {
        SixMonthsBeforeDue,
        GivenDate,
        DontHide,
    }

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

    [Fact]
    public void Map_TaskBasicProperties()
    {
        var gtdDataModel = CreateGTDDataModelWithTask();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var gtdTaskModel = gtdDataModel.Task![0];
        var taskAppTaskModel = taskAppDataModel?.Tasks?[0]!;
        var gtdRemappedTaskModel = gtdDataMappedRemappedModel?.Task?[0]!;

        AssertBasicTaskProperties(gtdTaskModel, taskAppTaskModel, gtdRemappedTaskModel);
    }

    [Fact]
    public void Map_TaskDates()
    {
        var gtdDataModel = CreateGTDDataModelWithTask();

        var (taskAppDataModel, _) = GetMappedInfo(gtdDataModel);
        var gtdTaskModel = gtdDataModel.Task![0];
        var taskAppTaskModel = taskAppDataModel?.Tasks?[0]!;

        AssertTaskDates(gtdTaskModel, taskAppTaskModel);
    }

    [Fact]
    public void Map_TaskKeywords()
    {
        var gtdDataModel = CreateGTDDataModelWithTask();

        var (taskAppDataModel, _) = GetMappedInfo(gtdDataModel);
        var gtdTaskModel = gtdDataModel.Task![0];
        var taskAppTaskModel = taskAppDataModel?.Tasks?[0]!;

        AssertTaskKeywords(gtdTaskModel, taskAppTaskModel);
    }

    [Fact]
    public void Map_TaskWithoutParent()
    {
        var gtdDataModel = CreateGTDDataModelWithTask();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var taskAppTaskModelWithoutParent = taskAppDataModel?.Tasks?[0]!;
        var gtdRemappedTaskModelWithoutParent = gtdDataMappedRemappedModel?.Task?[0]!;

        Assert.Null(taskAppTaskModelWithoutParent.Parent);
        Assert.Equal(0, gtdRemappedTaskModelWithoutParent.Parent);
    }

    [Fact]
    public void Map_TaskWithParent()
    {
        var gtdDataModel = CreateGTDDataModelWithTask([CreateGTDDataTaskModelBuilder(TestConstants.DefaultTaskId).WithParent(10), CreateGTDDataTaskModelBuilder(10)]);

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var taskAppTaskModel = taskAppDataModel?.Tasks?[0]!;
        var gtdRemappedTaskModelWithoutParent = gtdDataMappedRemappedModel?.Task?[0]!;

        Assert.Equal("10", taskAppTaskModel.Parent!.Id);
        Assert.Equal(10, gtdRemappedTaskModelWithoutParent.Parent);
    }

    [Fact]
    public void Map_TaskNote()
    {
        var gtdDataModel = CreateGTDDataModelWithTaskNote();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var gtdTaskNoteModel = gtdDataModel.TaskNote![0];
        var taskAppTaskNoteModel = taskAppDataModel?.TaskNotes?[0]!;
        var gtdRemappedTaskNoteModel = gtdDataMappedRemappedModel?.TaskNote?[0]!;

        AssertMappedModelEquivalence(gtdTaskNoteModel, taskAppTaskNoteModel, gtdRemappedTaskNoteModel);
    }

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

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Map_TaskWithDueDate(bool hasTime)
    {
        var dueDate = hasTime ? new LocalDateTime(2023, 02, 20, 10, 0, 0) : new LocalDateTime(2023, 02, 20, 0, 0, 0);
        var gtdDataModel = CreateGTDDataModelWithTask([CreateGTDDataTaskModelBuilder().WithDueDate(dueDate)]);

        var (_, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var gtdRemappedTaskModel = gtdDataMappedRemappedModel?.Task?[0]!;

        Assert.Equal(hasTime, gtdRemappedTaskModel.DueTimeSet);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Map_TaskWithDueDateModifier(bool isFloating)
    {
        var gtdDataModel = CreateGTDDataModelWithTask([CreateGTDDataTaskModelBuilder().WithFloating(isFloating)]);

        var (_, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var gtdRemappedTaskModel = gtdDataMappedRemappedModel?.Task?[0]!;

        Assert.Equal(isFloating ? DueDateModifier.OptionallyOn : DueDateModifier.DueBy, gtdRemappedTaskModel.DueDateModifier);
    }

    [Theory]
    [InlineData(ReminderTestCase.NoReminder, true, null)]
    [InlineData(ReminderTestCase.ImmediateReminder, true, BaseDateOfReminderInstant.FromDueDate)]
    [InlineData(ReminderTestCase.ShortReminder, true, BaseDateOfReminderInstant.FromDueDate)]
    [InlineData(ReminderTestCase.MediumReminder, true, BaseDateOfReminderInstant.FromDueDate)]
    [InlineData(ReminderTestCase.LongReminder, true, BaseDateOfReminderInstant.FromDueDate)]
    [InlineData(ReminderTestCase.AbsoluteReminder, true, BaseDateOfReminderInstant.FromUnixEpoch)]
    [InlineData(ReminderTestCase.SlightlyOverLongReminder, true, BaseDateOfReminderInstant.FromUnixEpoch)]
    [InlineData(ReminderTestCase.SlightlyUnderLongReminder, false, BaseDateOfReminderInstant.FromDueDate)]
    [InlineData(ReminderTestCase.SlightlyOverMediumReminder, false, BaseDateOfReminderInstant.FromDueDate)]
    public void Map_TaskWithReminder(ReminderTestCase reminderCase, bool expectOriginalValue, BaseDateOfReminderInstant? expectedBase)
    {
        var reminder = GetReminderValue(reminderCase);
        var dueDateInstant = Instant.FromUnixTimeMilliseconds(TestConstants.DefaultDueDateMilliseconds);
        var dueDate = dueDateInstant.GetLocalDateTime(CurrentDateTimeZone);
        var gtdDataModel = CreateGTDDataModelWithTask([CreateGTDDataTaskModelBuilder().WithDueDate(dueDate).WithReminder(reminder)]);

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var taskAppTaskModel = taskAppDataModel?.Tasks?[0]!;
        var gtdRemappedTaskModel = gtdDataMappedRemappedModel?.Task?[0]!;

        Assert.Equal(expectedBase, taskAppTaskModel.Reminder?.ReminderInstantType);
        var expectedReminder = expectOriginalValue ? reminder : dueDateInstant.Plus(-Duration.FromMinutes(reminder)).ToUnixTimeMilliseconds();
        Assert.Equal(expectedReminder, gtdRemappedTaskModel.Reminder);
    }

    [Fact]
    public void Map_TaskWithReminderWithDueDateBasedReminderWithoutDueDate_ShouldReturnMinusOne()
    {
        var gtdDataModel = CreateGTDDataModelWithTask([CreateGTDDataTaskModelBuilder().WithDueDate(null).WithReminder(30000)]);

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var taskAppTaskModel = taskAppDataModel?.Tasks?[0]!;
        var gtdRemappedTaskModel = gtdDataMappedRemappedModel?.Task?[0]!;

        Assert.Equal(BaseDateOfReminderInstant.FromDueDate, taskAppTaskModel.Reminder?.ReminderInstantType);
        Assert.Null(taskAppTaskModel.Reminder?.AbsoluteInstant);
        Assert.Equal(-1, gtdRemappedTaskModel.Reminder);
    }

    [Theory]
    [InlineData(-1, true)]
    [InlineData(0, true)]
    [InlineData(1, false)]
    public void Map_TaskWithAlarm(int addMinutes, bool shouldHaveAlarm)
    {
        var currentDateTime = TestConverter.Clock.GetCurrentInstant();
        var reminder = currentDateTime.Plus(Duration.FromMinutes(addMinutes)).ToUnixTimeMilliseconds();

        var gtdDataModel = CreateGTDDataModelWithTask([CreateGTDDataTaskModelBuilder().WithReminder(reminder)]);
        var (_, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var gtdRemappedTaskModel = gtdDataMappedRemappedModel?.Task?[0]!;

        var expectedReminder = shouldHaveAlarm ? reminder : (long?)null;
        Assert.Equal(shouldHaveAlarm, gtdRemappedTaskModel.Alarm.HasValue);
        Assert.Equal(expectedReminder, gtdRemappedTaskModel.Alarm?.InZoneLeniently(CurrentDateTimeZone).ToInstant().ToUnixTimeMilliseconds());
    }

    [Theory]
    [InlineData(RepeatTestCase.EveryDay, GTDRepeatFrom.FromDueDate, RepeatFrom.FromDueDate, GTDRepeatFrom.FromDueDate, 1, Model.Model.Period.Day, true)]
    [InlineData(RepeatTestCase.EveryTwoWeeks, GTDRepeatFrom.FromDueDate, RepeatFrom.FromDueDate, GTDRepeatFrom.FromDueDate, 2, Model.Model.Period.Week, true)]
    [InlineData(RepeatTestCase.EveryThreeMonths, GTDRepeatFrom.FromDueDate, RepeatFrom.FromDueDate, GTDRepeatFrom.FromDueDate, 3, Model.Model.Period.Month, true)]
    [InlineData(RepeatTestCase.EveryFourYears, GTDRepeatFrom.FromDueDate, RepeatFrom.FromDueDate, GTDRepeatFrom.FromDueDate, 4, Model.Model.Period.Year, true)]
    [InlineData(RepeatTestCase.EveryDay, GTDRepeatFrom.FromCompletion, RepeatFrom.FromCompletion, GTDRepeatFrom.FromCompletion, 1, Model.Model.Period.Day, true)]
    [InlineData(RepeatTestCase.EveryDayLowerCase, GTDRepeatFrom.FromDueDate, RepeatFrom.FromDueDate, GTDRepeatFrom.FromDueDate, 1, Model.Model.Period.Day, true)]
    [InlineData(RepeatTestCase.NoRepeat, GTDRepeatFrom.FromDueDate, null, GTDRepeatFrom.FromDueDate, null, null, false)]
    [InlineData(RepeatTestCase.NoRepeat, GTDRepeatFrom.FromCompletion, null, GTDRepeatFrom.FromDueDate, null, null, false)]
    public void Map_Repeat(
        RepeatTestCase repeatCase,
        GTDRepeatFrom repeatFrom,
        RepeatFrom? expectedTaskAppRepeatFrom,
        GTDRepeatFrom expectedRemappedRepeatFrom,
        int? expectedTaskAppInterval,
        Model.Model.Period? expectedTaskAppPeriod,
        bool expectTaskAppRepeatInfo
    )
    {
        var repeatInfoString = GetRepeatInfoString(repeatCase);
        var repeatNew = expectTaskAppRepeatInfo ? new GTDRepeatInfoModel(repeatInfoString!) : (GTDRepeatInfoModel?)null;
        var gtdDataModel = CreateGTDDataModelWithTask([CreateGTDDataTaskModelBuilder().WithRepeat(repeatNew, repeatFrom)]);

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var taskAppTaskModel = taskAppDataModel?.Tasks?[0]!;
        var gtdRemappedTaskModel = gtdDataMappedRemappedModel?.Task?[0]!;
        var taskAppRepeatInfo = taskAppTaskModel.RepeatInfo;

        Assert.Equal(expectTaskAppRepeatInfo, taskAppRepeatInfo.HasValue);
        Assert.Equal(expectedTaskAppRepeatFrom, taskAppRepeatInfo?.RepeatFrom);
        Assert.Equal(expectedTaskAppInterval, taskAppRepeatInfo?.Interval);
        Assert.Equal(expectedTaskAppPeriod, taskAppRepeatInfo?.Period);
        Assert.Equal(expectedRemappedRepeatFrom, gtdRemappedTaskModel.RepeatFrom);
        Assert.Equal(repeatInfoString?.ToLowerInvariant(), gtdRemappedTaskModel.RepeatNew?.ToString().ToLowerInvariant());
    }

    [Theory]
    [InlineData(HideTestCase.SixMonthsBeforeDue)]
    [InlineData(HideTestCase.GivenDate)]
    [InlineData(HideTestCase.DontHide)]
    public void Map_Hide(HideTestCase hideCase)
    {
        var (hideUntil, hide) = GetHideInfo(hideCase);
        var hideInMilliseconds = hideUntil.HasValue ? hideUntil.Value.ToUnixTimeMilliseconds() : 0;
        var dueDate = new LocalDateTime(2023, 02, 20, 10, 0, 0);
        var gtdDataModel = CreateGTDDataModelWithTask([CreateGTDDataTaskModelBuilder().WithDueDate(dueDate).WithHideUntil(hideInMilliseconds).WithHide(hide)]);

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var taskAppTaskModel = taskAppDataModel?.Tasks?[0]!;
        var gtdRemappedTaskModel = gtdDataMappedRemappedModel?.Task?[0]!;

        Assert.Equal(hideUntil, taskAppTaskModel.HideUntil);
        Assert.Equal(hideInMilliseconds, gtdRemappedTaskModel.HideUntil);
        Assert.Equal(hide, gtdRemappedTaskModel.Hide);
    }

    private (TaskAppDataModel? model, GTDDataModel? fromModel) GetMappedInfo(GTDDataModel gtdDataModel)
    {
        if (gtdDataModel == null)
            return (null, null);
        var taskAppDataModel = TestConverter.MapToModel(gtdDataModel);
        var gtdDataMappedRemappedModel = TestConverter.MapFromModel(taskAppDataModel);

        return (taskAppDataModel, gtdDataMappedRemappedModel);
    }

    private void AssertCommonProperties<T>(T gtdModel, ExtendedModel taskAppModel)
        where T : GTDExtendedModel
    {
        Assert.Equal(gtdModel.Id.ToString(), taskAppModel.Id);
        Assert.Equal(gtdModel.Created, taskAppModel.Created.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(gtdModel.Modified, taskAppModel.Modified.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(gtdModel.Title, taskAppModel.Title);
        Assert.Equal(Color.FromArgb(gtdModel.Color), taskAppModel.Color);
        Assert.Equal(gtdModel.Visible, taskAppModel.Visible);
    }

    private void AssertMappedModelEquivalence<T>(T originalModel, ExtendedModel taskAppModel, T remappedModel)
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

    private static void AssertBasicTaskProperties(GTDTaskModel gtdTaskModel, TaskModel taskAppTaskModel, GTDTaskModel gtdRemappedTaskModel)
    {
        Assert.Equal(gtdTaskModel.Id.ToString(), taskAppTaskModel.Id);
        Assert.Null(taskAppTaskModel.Parent);
        Assert.Equal(gtdTaskModel.Title, taskAppTaskModel.Title);
        Assert.Equal(Model.Model.Status.NextAction, taskAppTaskModel.Status);
        Assert.Equal(gtdTaskModel.Starred, taskAppTaskModel.Starred);
        Assert.Equal(Model.Model.Priority.Low, taskAppTaskModel.Priority);
        Assert.Equal(gtdTaskModel.Note, taskAppTaskModel.Note);
        Assert.Equal(Model.Model.TaskType.Task, taskAppTaskModel.Type);
        Assert.Equal(gtdTaskModel.Floating, taskAppTaskModel.HasFloatingDueDate);

        Assert.Equivalent(gtdTaskModel, gtdRemappedTaskModel);
    }

    private void AssertTaskDates(GTDTaskModel gtdTaskModel, TaskModel taskAppTaskModel)
    {
        Assert.Equal(gtdTaskModel.Created, taskAppTaskModel.Created.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(gtdTaskModel.Modified, taskAppTaskModel.Modified.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(gtdTaskModel.DueDate, taskAppTaskModel.DueDate);
        Assert.Equal(gtdTaskModel.DueDateProject, taskAppTaskModel.DueDateProject);
        Assert.Equal(gtdTaskModel.Completed, taskAppTaskModel.Completed);
        Assert.Equal(gtdTaskModel.HideUntil, taskAppTaskModel.HideUntil!.Value.ToUnixTimeMilliseconds());
    }

    private static void AssertTaskKeywords(GTDTaskModel gtdTaskModel, TaskModel taskAppTaskModel)
    {
        Assert.Equal(gtdTaskModel.Context.ToString(), GetFirstIdOfKeyWord(taskAppTaskModel, GTDKeyWordEnum.Context));
        Assert.Equal(gtdTaskModel.Folder.ToString(), GetFirstIdOfKeyWord(taskAppTaskModel, GTDKeyWordEnum.Folder));
        Assert.Equal(gtdTaskModel.Tag.Select(t => t.ToString()), taskAppTaskModel.KeyWords.Where(t => t.KeyWordType == KeyWordEnum.Tag).Select(t => t.Id));
    }

    private static string GetFirstIdOfKeyWord(TaskModel taskModel, KeyWordEnum keyWordEnum)
    {
        return taskModel.KeyWords.FirstOrDefault(t => t.KeyWordType == keyWordEnum)!.Id;
    }

    private static string? GetRepeatInfoString(RepeatTestCase repeatCase)
    {
        return repeatCase switch
        {
            RepeatTestCase.EveryDay => "Every 1 day",
            RepeatTestCase.EveryDayLowerCase => "every 1 day",
            RepeatTestCase.EveryTwoWeeks => "Every 2 weeks",
            RepeatTestCase.EveryThreeMonths => "Every 3 months",
            RepeatTestCase.EveryFourYears => "Every 4 years",
            RepeatTestCase.NoRepeat => null,
            _ => throw new ArgumentOutOfRangeException(nameof(repeatCase), repeatCase, null),
        };
    }

    private (Instant? hideUntil, Hide hide) GetHideInfo(HideTestCase hideCase)
    {
        return hideCase switch
        {
            HideTestCase.SixMonthsBeforeDue => (new LocalDateTime(2022, 08, 20, 10, 0, 0).InZoneLeniently(CurrentDateTimeZone).ToInstant(), Hide.SixMonthsBeforeDue),
            HideTestCase.GivenDate => (new LocalDateTime(2022, 05, 10, 10, 0, 0).InZoneLeniently(CurrentDateTimeZone).ToInstant(), Hide.GivenDate),
            HideTestCase.DontHide => (null, Hide.DontHide),
            _ => throw new ArgumentOutOfRangeException(nameof(hideCase), hideCase, null),
        };
    }

    private static long GetReminderValue(ReminderTestCase reminderCase)
    {
        return reminderCase switch
        {
            ReminderTestCase.NoReminder => -1,
            ReminderTestCase.ImmediateReminder => 0,
            ReminderTestCase.ShortReminder => 180,
            ReminderTestCase.MediumReminder => 1080,
            ReminderTestCase.LongReminder => 43200,
            ReminderTestCase.AbsoluteReminder => TestConstants.DefaultDueDateMilliseconds,
            ReminderTestCase.SlightlyOverLongReminder => 43201,
            ReminderTestCase.SlightlyUnderLongReminder => 43199,
            ReminderTestCase.SlightlyOverMediumReminder => 1081,
            _ => throw new ArgumentOutOfRangeException(nameof(reminderCase), reminderCase, null),
        };
    }

    private static GTDDataModel CreateGTDDataModel()
    {
        return Create.A.GTDDataModel().Build();
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

    private static GTDDataModel CreateGTDDataModelWithTaskNote()
    {
        return Create.A.GTDDataModel().AddTaskNote(TestConstants.DefaultTaskNoteId).Build();
    }

    private static GTDDataModel CreateGTDDataModelWithNotebook()
    {
        return Create.A.GTDDataModel().AddFolder(TestConstants.DefaultFolderId).AddNotebook(TestConstants.DefaultNotebookId, TestConstants.DefaultFolderId).Build();
    }

    private static GTDDataModel CreateGTDDataModelWithPreferences()
    {
        return Create.A.GTDDataModel().AddPreferences().Build();
    }

    private static GTDDataModel CreateGTDDataModelWithTask(List<GTDTaskModelBuilder>? taskList = null)
    {
        taskList ??= [CreateGTDDataTaskModelBuilder()];

        return Create
            .A.GTDDataModel()
            .AddFolder(TestConstants.DefaultFolderId)
            .AddContext(TestConstants.DefaultContextId)
            .AddTag(TestConstants.DefaultTagId)
            .AddTag(8)
            .AddTaskList(() => taskList.Select(t => t.Build()).ToList())
            .Build();
    }

    private static GTDTaskModelBuilder CreateGTDDataTaskModelBuilder(int? taskId = null)
    {
        return Create
            .A.GTDTaskModel(taskId ?? TestConstants.DefaultTaskId)
            .WithFolder(TestConstants.DefaultFolderId)
            .WithContext(TestConstants.DefaultContextId)
            .WithTags([TestConstants.DefaultTagId, 8]);
    }
}
