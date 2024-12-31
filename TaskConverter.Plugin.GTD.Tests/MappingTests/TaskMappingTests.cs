using NodaTime;
using TaskConverter.Commons.Utils;
using TaskConverter.Model.Mapper;
using TaskConverter.Model.Model;
using TaskConverter.Plugin.GTD.Mapper;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Tests.Utils;
using TaskConverterModel = TaskConverter.Model.Model;

namespace TaskConverter.Plugin.GTD.Tests.MappingTests;

public class TaskMappingTests(IConverter testConverter, IClock clock, IConverterDateTimeZoneProvider converterDateTimeZoneProvider)
    : BaseMappingTests(testConverter, clock, converterDateTimeZoneProvider)
{
    public enum HideTestCase
    {
        SixMonthsBeforeDue,
        GivenDate,
        DontHide,
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
    [InlineData(RepeatTestCase.EveryDay, GTDRepeatFrom.FromDueDate, RepeatFrom.FromDueDate, GTDRepeatFrom.FromDueDate, 1, TaskConverterModel.Period.Day, true)]
    [InlineData(RepeatTestCase.EveryTwoWeeks, GTDRepeatFrom.FromDueDate, RepeatFrom.FromDueDate, GTDRepeatFrom.FromDueDate, 2, TaskConverterModel.Period.Week, true)]
    [InlineData(RepeatTestCase.EveryThreeMonths, GTDRepeatFrom.FromDueDate, RepeatFrom.FromDueDate, GTDRepeatFrom.FromDueDate, 3, TaskConverterModel.Period.Month, true)]
    [InlineData(RepeatTestCase.EveryFourYears, GTDRepeatFrom.FromDueDate, RepeatFrom.FromDueDate, GTDRepeatFrom.FromDueDate, 4, TaskConverterModel.Period.Year, true)]
    [InlineData(RepeatTestCase.EveryDay, GTDRepeatFrom.FromCompletion, RepeatFrom.FromCompletion, GTDRepeatFrom.FromCompletion, 1, TaskConverterModel.Period.Day, true)]
    [InlineData(RepeatTestCase.EveryDayLowerCase, GTDRepeatFrom.FromDueDate, RepeatFrom.FromDueDate, GTDRepeatFrom.FromDueDate, 1, TaskConverterModel.Period.Day, true)]
    [InlineData(RepeatTestCase.NoRepeat, GTDRepeatFrom.FromDueDate, null, GTDRepeatFrom.FromDueDate, null, null, false)]
    [InlineData(RepeatTestCase.NoRepeat, GTDRepeatFrom.FromCompletion, null, GTDRepeatFrom.FromDueDate, null, null, false)]
    public void Map_Repeat(
        RepeatTestCase repeatCase,
        GTDRepeatFrom repeatFrom,
        RepeatFrom? expectedTaskAppRepeatFrom,
        GTDRepeatFrom expectedRemappedRepeatFrom,
        int? expectedTaskAppInterval,
        TaskConverterModel.Period? expectedTaskAppPeriod,
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

    private static void AssertBasicTaskProperties(GTDTaskModel gtdTaskModel, TaskModel taskAppTaskModel, GTDTaskModel gtdRemappedTaskModel)
    {
        Assert.Equal(gtdTaskModel.Id.ToString(), taskAppTaskModel.Id);
        Assert.Null(taskAppTaskModel.Parent);
        Assert.Equal(gtdTaskModel.Title, taskAppTaskModel.Title);
        Assert.Equal(TaskConverterModel.Status.NextAction, taskAppTaskModel.Status);
        Assert.Equal(gtdTaskModel.Starred, taskAppTaskModel.Starred);
        Assert.Equal(0, taskAppTaskModel.Priority);
        Assert.Equal(gtdTaskModel.Note, taskAppTaskModel.Note?.GetStringArray());
        Assert.Equal(gtdTaskModel.Floating, taskAppTaskModel.HasFloatingDueDate);

        Assert.Equivalent(gtdTaskModel, gtdRemappedTaskModel);
    }

    private void AssertTaskDates(GTDTaskModel gtdTaskModel, TaskModel taskAppTaskModel)
    {
        Assert.Equal(gtdTaskModel.Created, taskAppTaskModel.Created.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(gtdTaskModel.Modified, taskAppTaskModel.Modified.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(gtdTaskModel.DueDate, taskAppTaskModel.DueDate);
        Assert.Equal(gtdTaskModel.Completed, taskAppTaskModel.Completed);
        Assert.Equal(gtdTaskModel.HideUntil, taskAppTaskModel.HideUntil!.Value.ToUnixTimeMilliseconds());
    }

    private static void AssertTaskKeywords(GTDTaskModel gtdTaskModel, TaskModel taskAppTaskModel)
    {
        Assert.Equal(gtdTaskModel.Context.ToString(), GetFirstIdOfKeyWord(taskAppTaskModel, GTDKeyWordEnum.Context));
        Assert.Equal(gtdTaskModel.Folder.ToString(), GetFirstIdOfKeyWord(taskAppTaskModel, GTDKeyWordEnum.Folder));
        Assert.Equal(gtdTaskModel.Tag.Select(t => t.ToString()), taskAppTaskModel.KeyWords.Where(t => t.KeyWordType == KeyWordEnum.Tag).Select(t => t.Id));

        string GetFirstIdOfKeyWord(TaskModel taskModel, KeyWordEnum keyWordEnum)
        {
            return taskModel.KeyWords.FirstOrDefault(t => t.KeyWordType == keyWordEnum)!.Id;
        }
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
