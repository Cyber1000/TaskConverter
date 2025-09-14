using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using NodaTime;
using TaskConverter.Commons;
using TaskConverter.Commons.Utils;
using TaskConverter.Plugin.GTD.Conversion;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Tests.Utils;
using TaskConverter.Plugin.GTD.TodoModel;
using TaskConverter.Plugin.GTD.Utils;

namespace TaskConverter.Plugin.GTD.Tests.MappingTests;

public class TaskMappingTests(IConversionService<GTDDataModel> testConverter, IClock clock, ISettingsProvider settingsProvider, IKeyWordMapperService keyWordMapperService)
    : BaseMappingTests(testConverter, clock, settingsProvider)
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
        var taskAppTaskModel = GetTodoById(taskAppDataModel, TestConstants.DefaultTaskId.ToString())!;
        var gtdRemappedTaskModel = GetTaskById(gtdDataMappedRemappedModel, TestConstants.DefaultTaskId)!;

        AssertBasicTaskProperties(gtdTaskModel, taskAppTaskModel, gtdRemappedTaskModel);
    }

    [Fact]
    public void Map_TaskDates()
    {
        var gtdDataModel = CreateGTDDataModelWithTask();

        var (taskAppDataModel, _) = GetMappedInfo(gtdDataModel);
        var gtdTaskModel = gtdDataModel.Task![0];
        var taskAppTaskModel = GetTodoById(taskAppDataModel, TestConstants.DefaultTaskId.ToString())!;

        AssertTaskDates(gtdTaskModel, taskAppTaskModel);
    }

    [Fact]
    public void Map_TaskKeywords()
    {
        var gtdDataModel = CreateGTDDataModelWithTask();

        var (taskAppDataModel, _) = GetMappedInfo(gtdDataModel);
        var gtdTaskModel = gtdDataModel.Task![0];

        AssertTaskKeywords(gtdTaskModel, taskAppDataModel!);
    }

    [Fact]
    public void Map_TaskWithoutParent()
    {
        var gtdDataModel = CreateGTDDataModelWithTask();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var taskAppTaskModelWithoutParent = GetTodoById(taskAppDataModel, TestConstants.DefaultTaskId.ToString())!;
        var gtdRemappedTaskModelWithoutParent = GetTaskById(gtdDataMappedRemappedModel, TestConstants.DefaultTaskId)!;

        Assert.IsType<Calendar>(taskAppTaskModelWithoutParent.Parent);
        Assert.Equal(0, gtdRemappedTaskModelWithoutParent.Parent);
    }

    [Fact]
    public void Map_TaskWithParent()
    {
        var gtdDataModel = CreateGTDDataModelWithTask([CreateGTDDataTaskModelBuilder(TestConstants.DefaultTaskId).WithParent(10), CreateGTDDataTaskModelBuilder(10)]);

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var taskAppTaskModel = GetTodoById(taskAppDataModel, TestConstants.DefaultTaskId.ToString())!;
        var gtdRemappedTaskModelWithoutParent = GetTaskById(gtdDataMappedRemappedModel, TestConstants.DefaultTaskId)!;

        Assert.IsType<Todo>(taskAppTaskModel.Parent);
        Assert.Equal("10", (taskAppTaskModel.Parent as Todo)!.Uid);
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
        var gtdRemappedTaskModel = GetTaskById(gtdDataMappedRemappedModel, TestConstants.DefaultTaskId)!;

        Assert.Equal(hasTime, gtdRemappedTaskModel.DueTimeSet);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Map_TaskWithDueDateModifier(bool isFloating)
    {
        var gtdDataModel = CreateGTDDataModelWithTask([CreateGTDDataTaskModelBuilder().WithFloating(isFloating)]);

        var (_, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var gtdRemappedTaskModel = GetTaskById(gtdDataMappedRemappedModel, TestConstants.DefaultTaskId)!;

        Assert.Equal(isFloating ? DueDateModifier.OptionallyOn : DueDateModifier.DueBy, gtdRemappedTaskModel.DueDateModifier);
    }

    [Theory]
    [InlineData(ReminderTestCase.NoReminder, true, false, false)]
    [InlineData(ReminderTestCase.ImmediateReminder, true, false, true)]
    [InlineData(ReminderTestCase.ShortReminder, true, false, true)]
    [InlineData(ReminderTestCase.MediumReminder, true, false, true)]
    [InlineData(ReminderTestCase.LongReminder, true, false, true)]
    [InlineData(ReminderTestCase.AbsoluteReminder, true, true, false)]
    [InlineData(ReminderTestCase.SlightlyOverLongReminder, true, true, false)]
    [InlineData(ReminderTestCase.SlightlyUnderLongReminder, false, false, true)]
    [InlineData(ReminderTestCase.SlightlyOverMediumReminder, false, false, true)]
    public void Map_TaskWithReminder(ReminderTestCase reminderCase, bool expectOriginalValue, bool unixDateTimeHasValue, bool durationHasValue)
    {
        var reminder = GetReminderValue(reminderCase);
        var dueDateInstant = Instant.FromUnixTimeMilliseconds(TestConstants.DefaultDueDateMilliseconds);
        var dueDate = dueDateInstant.GetLocalDateTime(CurrentDateTimeZone);
        var gtdDataModel = CreateGTDDataModelWithTask([CreateGTDDataTaskModelBuilder().WithDueDate(dueDate).WithReminder(reminder)]);

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var taskAppTaskModel = GetTodoById(taskAppDataModel, TestConstants.DefaultTaskId.ToString())!;
        var gtdRemappedTaskModel = GetTaskById(gtdDataMappedRemappedModel, TestConstants.DefaultTaskId)!;

        Assert.Equal(unixDateTimeHasValue, taskAppTaskModel.Alarms.FirstOrDefault()?.Trigger.DateTime != null);
        Assert.Equal(durationHasValue, taskAppTaskModel.Alarms.FirstOrDefault()?.Trigger.Duration != null);
        var expectedReminder = expectOriginalValue ? reminder : dueDateInstant.Plus(-Duration.FromMinutes(reminder)).ToUnixTimeMilliseconds();
        Assert.Equal(expectedReminder, gtdRemappedTaskModel.Reminder);
    }

    [Fact]
    public void Map_TaskWithReminderWithDueDateBasedReminderWithoutDueDate_ShouldReturnMinusOne()
    {
        var gtdDataModel = CreateGTDDataModelWithTask([CreateGTDDataTaskModelBuilder().WithDueDate(null).WithReminder(30000)]);

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var taskAppTaskModel = GetTodoById(taskAppDataModel, TestConstants.DefaultTaskId.ToString())!;
        var gtdRemappedTaskModel = GetTaskById(gtdDataMappedRemappedModel, TestConstants.DefaultTaskId)!;

        Assert.Null(taskAppTaskModel.Alarms.FirstOrDefault()?.Trigger.DateTime);
        Assert.NotNull(taskAppTaskModel.Alarms.FirstOrDefault()?.Trigger.Duration);

        Assert.Equal(-1, gtdRemappedTaskModel.Reminder);
    }

    [Theory]
    [InlineData(-1, true)]
    [InlineData(0, true)]
    [InlineData(1, false)]
    public void Map_TaskWithAlarm(int addMinutes, bool shouldHaveAlarm)
    {
        var currentDateTime = clock.GetCurrentInstant();
        var reminder = currentDateTime.Plus(Duration.FromMinutes(addMinutes)).ToUnixTimeMilliseconds();

        var gtdDataModel = CreateGTDDataModelWithTask([CreateGTDDataTaskModelBuilder().WithReminder(reminder)]);
        var (_, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var gtdRemappedTaskModel = GetTaskById(gtdDataMappedRemappedModel, TestConstants.DefaultTaskId)!;

        var expectedReminder = shouldHaveAlarm ? reminder : (long?)null;
        Assert.Equal(shouldHaveAlarm, gtdRemappedTaskModel.Alarm.HasValue);
        Assert.Equal(expectedReminder, gtdRemappedTaskModel.Alarm?.InZoneLeniently(CurrentDateTimeZone).ToInstant().ToUnixTimeMilliseconds());
    }

    [Theory]
    [InlineData(RepeatTestCase.EveryDay, GTDRepeatFrom.FromDueDate, true, GTDRepeatFrom.FromDueDate, 1, FrequencyType.Daily, true)]
    [InlineData(RepeatTestCase.EveryTwoWeeks, GTDRepeatFrom.FromDueDate, true, GTDRepeatFrom.FromDueDate, 2, FrequencyType.Weekly, true)]
    [InlineData(RepeatTestCase.EveryThreeMonths, GTDRepeatFrom.FromDueDate, true, GTDRepeatFrom.FromDueDate, 3, FrequencyType.Monthly, true)]
    [InlineData(RepeatTestCase.EveryFourYears, GTDRepeatFrom.FromDueDate, true, GTDRepeatFrom.FromDueDate, 4, FrequencyType.Yearly, true)]
    [InlineData(RepeatTestCase.EveryDay, GTDRepeatFrom.FromCompletion, false, GTDRepeatFrom.FromCompletion, 1, FrequencyType.Daily, true)]
    [InlineData(RepeatTestCase.EveryDayLowerCase, GTDRepeatFrom.FromDueDate, true, GTDRepeatFrom.FromDueDate, 1, FrequencyType.Daily, true)]
    [InlineData(RepeatTestCase.NoRepeat, GTDRepeatFrom.FromDueDate, null, GTDRepeatFrom.FromDueDate, null, null, false)]
    [InlineData(RepeatTestCase.NoRepeat, GTDRepeatFrom.FromCompletion, null, GTDRepeatFrom.FromDueDate, null, null, false)]
    public void Map_Repeat(
        RepeatTestCase repeatCase,
        GTDRepeatFrom repeatFrom,
        bool? repeatFromDueDate,
        GTDRepeatFrom expectedRemappedRepeatFrom,
        int? expectedTaskAppInterval,
        FrequencyType? expectedTaskAppPeriod,
        bool expectTaskAppRepeatInfo
    )
    {
        var repeatInfoString = GetRepeatInfoString(repeatCase);
        var repeatNew = expectTaskAppRepeatInfo ? new GTDRepeatInfoModel(repeatInfoString!) : (GTDRepeatInfoModel?)null;
        var gtdDataModel = CreateGTDDataModelWithTask([CreateGTDDataTaskModelBuilder().WithRepeat(repeatNew, repeatFrom)]);

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var taskAppTaskModel = GetTodoById(taskAppDataModel, TestConstants.DefaultTaskId.ToString())!;
        var gtdRemappedTaskModel = GetTaskById(gtdDataMappedRemappedModel, TestConstants.DefaultTaskId)!;
        var taskAppRepeatInfo = taskAppTaskModel.RecurrenceRules.FirstOrDefault();

        Assert.Equal(expectTaskAppRepeatInfo, taskAppRepeatInfo != null);
        var expectedStartDate = GetExpectedStartDate(repeatFromDueDate, taskAppTaskModel);
        Assert.Equal(expectedStartDate, taskAppTaskModel?.Start);
        Assert.Equal(expectedTaskAppInterval, taskAppRepeatInfo?.Interval);
        Assert.Equal(expectedTaskAppPeriod, taskAppRepeatInfo?.Frequency);
        Assert.Equal(expectedRemappedRepeatFrom, gtdRemappedTaskModel.RepeatFrom);
        Assert.Equal(repeatInfoString?.ToLowerInvariant(), gtdRemappedTaskModel.RepeatNew?.ToString().ToLowerInvariant());
    }

    [Fact]
    public void Map_MultipleRecurrencesWithFalseAllowIncompleteMappingIfMoreThanOneItem_ThrowsException()
    {
        ((TestSettingsProvider)TestConverter.SettingsProvider).AllowIncompleteMappingIfMoreThanOneItem = false;

        var todo = Create.A.Todo().AddRecurrenceRule(new RecurrencePattern(FrequencyType.Daily, 5)).AddRecurrenceRule(new RecurrencePattern(FrequencyType.Weekly, 10)).Build();
        var calendar = Create.A.Calendar().WithTask(todo).Build();

        var exception = Assert.Throws<Exception>(() => TestConverter.MapFromIntermediateFormat(calendar));
        Assert.Equal("More than one RecurrenceRule. This is only allowed if AllowIncompleteMappingIfMoreThanOneItem is true.", exception.Message);
    }

    [Fact]
    public void Map_MultipleRecurrencesWithTrueAllowIncompleteMappingIfMoreThanOneItem_DoesNotThrow()
    {
        ((TestSettingsProvider)TestConverter.SettingsProvider).AllowIncompleteMappingIfMoreThanOneItem = true;

        var todo = Create.A.Todo().AddRecurrenceRule(new RecurrencePattern(FrequencyType.Daily, 5)).AddRecurrenceRule(new RecurrencePattern(FrequencyType.Weekly, 10)).Build();
        var calendar = Create.A.Calendar().WithTask(todo).Build();

        var ex = Record.Exception(() => TestConverter.MapFromIntermediateFormat(calendar));
        Assert.Null(ex);
    }

    [Fact]
    public void Map_MultipleAlarmsWithFalseAllowIncompleteMappingIfMoreThanOneItem_ThrowsException()
    {
        ((TestSettingsProvider)TestConverter.SettingsProvider).AllowIncompleteMappingIfMoreThanOneItem = false;

        var todo = Create.A.Todo().AddAlarm(new Alarm()).AddAlarm(new Alarm()).Build();
        var calendar = Create.A.Calendar().WithTask(todo).Build();

        var exception = Assert.Throws<AutoMapper.AutoMapperMappingException>(() => TestConverter.MapFromIntermediateFormat(calendar));
        Assert.Equal("More than one Alarm. This is only allowed if AllowIncompleteMappingIfMoreThanOneItem is true.", exception.InnerException!.Message);
    }

    [Fact]
    public void Map_MultipleAlarmsWithTrueAllowIncompleteMappingIfMoreThanOneItem_DoesNotThrow()
    {
        ((TestSettingsProvider)TestConverter.SettingsProvider).AllowIncompleteMappingIfMoreThanOneItem = true;

        var todo = Create.A.Todo().AddAlarm(new Alarm()).AddAlarm(new Alarm()).Build();
        var calendar = Create.A.Calendar().WithTask(todo).Build();

        var ex = Record.Exception(() => TestConverter.MapFromIntermediateFormat(calendar));
        Assert.Null(ex);
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
        var taskAppTaskModel = GetTodoById(taskAppDataModel, TestConstants.DefaultTaskId.ToString())!;
        var gtdRemappedTaskModel = GetTaskById(gtdDataMappedRemappedModel, TestConstants.DefaultTaskId)!;

        var taskAppTaskModelHideUntil = taskAppTaskModel.Properties.Get<CalDateTime>(IntermediateFormatPropertyNames.HideUntil);
        Assert.Equal(hideUntil, taskAppTaskModelHideUntil?.GetInstant());
        Assert.Equal(hideInMilliseconds, gtdRemappedTaskModel.HideUntil);
        Assert.Equal(hide, gtdRemappedTaskModel.Hide);
    }

    private static void AssertBasicTaskProperties(GTDTaskModel gtdTaskModel, Todo taskAppTaskModel, GTDTaskModel gtdRemappedTaskModel)
    {
        Assert.Equal(gtdTaskModel.Id.ToString(), taskAppTaskModel.Uid);
        Assert.IsType<Calendar>(taskAppTaskModel.Parent);
        Assert.Equal(gtdTaskModel.Title, taskAppTaskModel.Summary);
        Assert.Equal("NextAction", taskAppTaskModel.Status);
        var starred = bool.Parse(taskAppTaskModel.Properties.Get<string>(IntermediateFormatPropertyNames.Starred));
        Assert.Equal(gtdTaskModel.Starred, starred);
        Assert.Equal(0, taskAppTaskModel.Priority);
        Assert.Equal(gtdTaskModel.Note, taskAppTaskModel.Description?.GetStringArray());
        var floating = bool.Parse(taskAppTaskModel.Properties.Get<string>(IntermediateFormatPropertyNames.DueFloat));
        Assert.Equal(gtdTaskModel.Floating, floating);
        Assert.Equivalent(gtdTaskModel, gtdRemappedTaskModel);
    }

    private void AssertTaskDates(GTDTaskModel gtdTaskModel, Todo taskAppTaskModel)
    {
        Assert.Equal(gtdTaskModel.Created, taskAppTaskModel.Created.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(gtdTaskModel.Modified, taskAppTaskModel.LastModified.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(gtdTaskModel.DueDate, taskAppTaskModel.Due.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(gtdTaskModel.Completed, taskAppTaskModel.Completed.GetLocalDateTime(CurrentDateTimeZone));
        var hideUntil = taskAppTaskModel.Properties.Get<CalDateTime>(IntermediateFormatPropertyNames.HideUntil);
        var hideUntilMilliseconds = new DateTimeOffset(hideUntil.Value).ToUnixTimeMilliseconds();
        Assert.Equal(gtdTaskModel.HideUntil, hideUntilMilliseconds);
    }

    private void AssertTaskKeywords(GTDTaskModel gtdTaskModel, Calendar taskAppDataModel)
    {
        var keyWordMetaDataList = keyWordMapperService.GetKeyWordMetaDataIntermediateFormatDictionary(taskAppDataModel!, CurrentDateTimeZone).Values;
        Assert.Equal(gtdTaskModel.Context, GetFirstIdOfKeyWord(KeyWordType.Context));
        Assert.Equal(gtdTaskModel.Folder, GetFirstIdOfKeyWord(KeyWordType.Folder));
        Assert.Equal(gtdTaskModel.Tag.Select(t => t), keyWordMetaDataList.Where(t => t.KeyWordType == KeyWordType.Tag).Select(t => t.Id));

        int GetFirstIdOfKeyWord(KeyWordType keyWordEnum)
        {
            return keyWordMetaDataList.FirstOrDefault(t => t.KeyWordType == keyWordEnum)!.Id;
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

    private static IDateTime? GetExpectedStartDate(bool? repeatFromDueDate, Todo? taskAppTaskModel)
    {
        if (repeatFromDueDate == null)
            return null;

        return repeatFromDueDate.Value ? taskAppTaskModel?.Due : taskAppTaskModel?.Completed;
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

    private static Todo? GetTodoById(Calendar? calendar, string todoId)
    {
        if (calendar == null)
            return null;

        return FindTodoById(calendar, todoId);

        static Todo? FindTodoById(ICalendarObject calendarobject, string todoId)
        {
            foreach (var calendarItem in calendarobject.Children)
            {
                if (calendarItem is Todo todo && todo.Uid == todoId)
                    return todo;
                else
                {
                    var innerTodo = FindTodoById(calendarItem, todoId);
                    if (innerTodo != null)
                        return innerTodo;
                }
            }
            return null;
        }
    }

    private static GTDTaskModel? GetTaskById(GTDDataModel? dataModel, int taskId)
    {
        if (dataModel?.Task == null)
            return null;

        foreach (var task in dataModel.Task)
        {
            if (task.Id == taskId)
                return task;
        }
        return null;
    }
}
