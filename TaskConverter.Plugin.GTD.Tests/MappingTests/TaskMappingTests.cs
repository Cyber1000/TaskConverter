using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using NodaTime;
using TaskConverter.Plugin.Base;
using TaskConverter.Plugin.Base.Utils;
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
        OverLongReminder,
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
    public void Map_TaskKeywordsWithSameNames_ShouldMapCorrectly()
    {
        ((TestSettingsProvider)TestConverter.SettingsProvider).SetIntermediateFormatSymbol(KeyWordType.Folder, "+");
        ((TestSettingsProvider)TestConverter.SettingsProvider).SetIntermediateFormatSymbol(KeyWordType.Context, "@");
        ((TestSettingsProvider)TestConverter.SettingsProvider).SetIntermediateFormatSymbol(KeyWordType.Status, "#");
        ((TestSettingsProvider)TestConverter.SettingsProvider).SetGTDFormatSymbol(KeyWordType.Folder, "");
        ((TestSettingsProvider)TestConverter.SettingsProvider).SetGTDFormatSymbol(KeyWordType.Context, "@");
        ((TestSettingsProvider)TestConverter.SettingsProvider).SetGTDFormatSymbol(KeyWordType.Status, "#");

        var gtdDataModel = CreateGTDDataModelWithTask();
        gtdDataModel.Folder!.First().Title = "Test";
        gtdDataModel.Context!.First().Title = "Test";
        gtdDataModel.Tag!.First().Title = "Test";

        var (taskAppDataModel, _) = GetMappedInfo(gtdDataModel);
        var gtdTaskModel = gtdDataModel.Task![0];

        AssertTaskKeywords(gtdTaskModel, taskAppDataModel!);
    }

    [Theory]
    [InlineData(Status.None, false, "NEEDS-ACTION", null)]
    [InlineData(Status.NextAction, false, "IN-PROCESS", "#NextAction")]
    [InlineData(Status.Active, false, "IN-PROCESS", "#Active")]
    [InlineData(Status.Planning, false, "IN-PROCESS", "#Planning")]
    [InlineData(Status.Delegated, false, "IN-PROCESS", "#Delegated")]
    [InlineData(Status.Waiting, false, "NEEDS-ACTION", "#Waiting")]
    [InlineData(Status.Hold, false, "NEEDS-ACTION", "#Hold")]
    [InlineData(Status.Postponed, false, "NEEDS-ACTION", "#Postponed")]
    [InlineData(Status.Someday, false, "NEEDS-ACTION", "#Someday")]
    [InlineData(Status.Canceled, false, "CANCELLED", null)]
    [InlineData(Status.Reference, false, "IN-PROCESS", "#Reference")]
    public void Map_State_ShouldBeValid(Status status, bool isCompleted, string expectedStatus, string expectedCategory)
    {
        ((TestSettingsProvider)TestConverter.SettingsProvider).SetIntermediateFormatSymbol(KeyWordType.Folder, "+");
        ((TestSettingsProvider)TestConverter.SettingsProvider).SetIntermediateFormatSymbol(KeyWordType.Context, "@");
        ((TestSettingsProvider)TestConverter.SettingsProvider).SetIntermediateFormatSymbol(KeyWordType.Status, "#");
        ((TestSettingsProvider)TestConverter.SettingsProvider).SetGTDFormatSymbol(KeyWordType.Folder, "");
        ((TestSettingsProvider)TestConverter.SettingsProvider).SetGTDFormatSymbol(KeyWordType.Context, "@");
        ((TestSettingsProvider)TestConverter.SettingsProvider).SetGTDFormatSymbol(KeyWordType.Status, "#");

        var gtdDataModel = CreateGTDDataModelWithTask();
        var task = gtdDataModel.Task!.First();
        task.Status = status;
        task.Completed = isCompleted ? new LocalDateTime(2023, 02, 25, 10, 0, 0) : null;
        var taskAppDataModel = TestConverter.MapToIntermediateFormat(gtdDataModel);
        Assert.Equal(expectedStatus, taskAppDataModel.Todos.First().Status);

        var categories = taskAppDataModel.Todos.First().Categories;
        var properties = taskAppDataModel.Todos.First().Properties.ToDictionary(n => n.Name);

        if (expectedCategory == null)
            Assert.DoesNotContain(categories, c => c.StartsWith('#'));
        else
        {
            Assert.Single(categories, c => c == expectedCategory);
            Assert.True(properties.TryGetValue(IntermediateFormatPropertyNames.CategoryMetaData(expectedCategory), out var prop));
            Assert.True(prop.Value is KeyWordMetaData existingMeta);
            Assert.Equal(KeyWordType.Status, ((KeyWordMetaData)prop.Value).KeyWordType);
        }
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

        var parentProp = taskAppTaskModel.Properties.FirstOrDefault(p =>
            string.Equals(p.Name, "RELATED-TO", StringComparison.OrdinalIgnoreCase)
            && (
                p.Parameters.Any(param => string.Equals(param.Name, "RELTYPE", StringComparison.OrdinalIgnoreCase) && string.Equals(param.Value, "PARENT", StringComparison.OrdinalIgnoreCase))
                || !p.Parameters.Any(param => string.Equals(param.Name, "RELTYPE", StringComparison.OrdinalIgnoreCase))
            )
        );
        Assert.NotNull(parentProp);

        Assert.Equal("10", parentProp.Value as string);
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
    [InlineData(ReminderTestCase.OverLongReminder, true, true, false)]
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

        Assert.Equal(unixDateTimeHasValue, taskAppTaskModel.Alarms.FirstOrDefault()?.Trigger!.DateTime != null);
        Assert.Equal(durationHasValue, taskAppTaskModel.Alarms.FirstOrDefault()?.Trigger!.Duration != null);
        var expectedReminder = expectOriginalValue ? reminder : dueDateInstant.Plus(-NodaTime.Duration.FromMinutes(reminder)).ToUnixTimeMilliseconds();
        Assert.Equal(expectedReminder, gtdRemappedTaskModel.Reminder);
    }

    [Fact]
    public void Map_SlightlyOverLongReminder_ShouldThrowException()
    {
        var reminder = GetReminderValue(ReminderTestCase.SlightlyOverLongReminder);
        var gtdDataModel = CreateGTDDataModelWithTask([CreateGTDDataTaskModelBuilder().WithReminder(reminder)]);

        var exception = Assert.Throws<ArgumentException>(() => GetMappedInfo(gtdDataModel));
        Assert.Equal("Reminder must be a multiple of 1000", exception.Message);
    }

    [Fact]
    public void Map_TaskWithReminderWithDueDateBasedReminderWithoutDueDate_ShouldReturnMinusOne()
    {
        var gtdDataModel = CreateGTDDataModelWithTask([CreateGTDDataTaskModelBuilder().WithDueDate(null).WithReminder(30000)]);

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var taskAppTaskModel = GetTodoById(taskAppDataModel, TestConstants.DefaultTaskId.ToString())!;
        var gtdRemappedTaskModel = GetTaskById(gtdDataMappedRemappedModel, TestConstants.DefaultTaskId)!;

        Assert.Null(taskAppTaskModel.Alarms.FirstOrDefault()?.Trigger!.DateTime);
        Assert.NotNull(taskAppTaskModel.Alarms.FirstOrDefault()?.Trigger!.Duration);

        Assert.Equal(-1, gtdRemappedTaskModel.Reminder);
    }

    [Theory]
    [InlineData(-1, true)]
    [InlineData(0, true)]
    [InlineData(1, false)]
    public void Map_TaskWithAlarm(int addMinutes, bool shouldHaveAlarm)
    {
        var currentDateTime = clock.GetCurrentInstant();
        var reminder = currentDateTime.Plus(NodaTime.Duration.FromMinutes(addMinutes)).ToUnixTimeMilliseconds();

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
    public void Map_RepeatFromDueDateWithoutDueDate_TestFallbackToCompletedDate()
    {
        var repeatInfoString = GetRepeatInfoString(RepeatTestCase.EveryDay);
        var repeatNew = new GTDRepeatInfoModel(repeatInfoString!);
        var gtdDataModel = CreateGTDDataModelWithTask([CreateGTDDataTaskModelBuilder().WithRepeat(repeatNew, GTDRepeatFrom.FromDueDate).WithoutDueDate()]);
        var gtdTaskModel = gtdDataModel.Task!.First();

        var (taskAppDataModel, _) = GetMappedInfo(gtdDataModel);
        var taskAppTaskModel = GetTodoById(taskAppDataModel, TestConstants.DefaultTaskId.ToString())!;

        Assert.Equal(gtdTaskModel.Completed.GetCalDateTime(CurrentDateTimeZone), taskAppTaskModel?.Start);
    }

    [Fact]
    public void Map_RepeatFromDueDateWithoutDueDateAndCompletedDate_TestFallbackToStartDate()
    {
        var repeatInfoString = GetRepeatInfoString(RepeatTestCase.EveryDay);
        var repeatNew = new GTDRepeatInfoModel(repeatInfoString!);
        var gtdDataModel = CreateGTDDataModelWithTask([CreateGTDDataTaskModelBuilder().WithRepeat(repeatNew, GTDRepeatFrom.FromDueDate).WithoutDueDate().WithoutCompletedDate()]);
        var gtdTaskModel = gtdDataModel.Task!.First();

        var (taskAppDataModel, _) = GetMappedInfo(gtdDataModel);
        var taskAppTaskModel = GetTodoById(taskAppDataModel, TestConstants.DefaultTaskId.ToString())!;

        Assert.Equal(gtdTaskModel.StartDate.GetCalDateTime(CurrentDateTimeZone), taskAppTaskModel?.Start);
    }

    [Fact]
    public void Map_RepeatFromDueDateWithoutDueDateAndCompletedDateAndStartDate_TestFallbackToCreated()
    {
        var repeatInfoString = GetRepeatInfoString(RepeatTestCase.EveryDay);
        var repeatNew = new GTDRepeatInfoModel(repeatInfoString!);
        var gtdDataModel = CreateGTDDataModelWithTask([CreateGTDDataTaskModelBuilder().WithRepeat(repeatNew, GTDRepeatFrom.FromDueDate).WithoutDueDate().WithoutCompletedDate().WithoutStartDate()]);
        var gtdTaskModel = gtdDataModel.Task!.First();

        var (taskAppDataModel, _) = GetMappedInfo(gtdDataModel);
        var taskAppTaskModel = GetTodoById(taskAppDataModel, TestConstants.DefaultTaskId.ToString())!;

        Assert.Equal(gtdTaskModel.Created.GetCalDateTime(CurrentDateTimeZone), taskAppTaskModel?.Start);
    }

    [Fact]
    public void Map_RepeatFromCompletionWithoutDueDate_UsesCompletedDate()
    {
        var repeatInfoString = GetRepeatInfoString(RepeatTestCase.EveryDay);
        var repeatNew = new GTDRepeatInfoModel(repeatInfoString!);
        var gtdDataModel = CreateGTDDataModelWithTask([CreateGTDDataTaskModelBuilder().WithRepeat(repeatNew, GTDRepeatFrom.FromCompletion).WithoutDueDate()]);
        var gtdTaskModel = gtdDataModel.Task!.First();

        var (taskAppDataModel, _) = GetMappedInfo(gtdDataModel);
        var taskAppTaskModel = GetTodoById(taskAppDataModel, TestConstants.DefaultTaskId.ToString())!;

        Assert.Equal(gtdTaskModel.Completed.GetCalDateTime(CurrentDateTimeZone), taskAppTaskModel?.Start);
    }

    [Fact]
    public void Map_RepeatFromCompletionWithoutCompletedDate_TestFallbackToDueDate()
    {
        var repeatInfoString = GetRepeatInfoString(RepeatTestCase.EveryDay);
        var repeatNew = new GTDRepeatInfoModel(repeatInfoString!);
        var gtdDataModel = CreateGTDDataModelWithTask([CreateGTDDataTaskModelBuilder().WithRepeat(repeatNew, GTDRepeatFrom.FromCompletion).WithoutCompletedDate()]);
        var gtdTaskModel = gtdDataModel.Task!.First();

        var (taskAppDataModel, _) = GetMappedInfo(gtdDataModel);
        var taskAppTaskModel = GetTodoById(taskAppDataModel, TestConstants.DefaultTaskId.ToString())!;

        Assert.Equal(gtdTaskModel.DueDate.GetCalDateTime(CurrentDateTimeZone), taskAppTaskModel?.Start);
    }

    [Fact]
    public void Map_RepeatFromCompletionWithoutCompletedDateAndDueDate_TestFallbackToStartDate()
    {
        var repeatInfoString = GetRepeatInfoString(RepeatTestCase.EveryDay);
        var repeatNew = new GTDRepeatInfoModel(repeatInfoString!);
        var gtdDataModel = CreateGTDDataModelWithTask([CreateGTDDataTaskModelBuilder().WithRepeat(repeatNew, GTDRepeatFrom.FromCompletion).WithoutCompletedDate().WithoutDueDate()]);
        var gtdTaskModel = gtdDataModel.Task!.First();

        var (taskAppDataModel, _) = GetMappedInfo(gtdDataModel);
        var taskAppTaskModel = GetTodoById(taskAppDataModel, TestConstants.DefaultTaskId.ToString())!;

        Assert.Equal(gtdTaskModel.StartDate.GetCalDateTime(CurrentDateTimeZone), taskAppTaskModel?.Start);
    }

    [Fact]
    public void Map_RepeatFromCompletionWithoutCompletedDateAndDueDateAndStartDate_TestFallbackToCreatedDate()
    {
        var repeatInfoString = GetRepeatInfoString(RepeatTestCase.EveryDay);
        var repeatNew = new GTDRepeatInfoModel(repeatInfoString!);
        var gtdDataModel = CreateGTDDataModelWithTask([CreateGTDDataTaskModelBuilder().WithRepeat(repeatNew, GTDRepeatFrom.FromCompletion).WithoutCompletedDate().WithoutDueDate().WithoutStartDate()]);
        var gtdTaskModel = gtdDataModel.Task!.First();

        var (taskAppDataModel, _) = GetMappedInfo(gtdDataModel);
        var taskAppTaskModel = GetTodoById(taskAppDataModel, TestConstants.DefaultTaskId.ToString())!;

        Assert.Equal(gtdTaskModel.Created.GetCalDateTime(CurrentDateTimeZone), taskAppTaskModel?.Start);
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

    [Fact]
    public void Map_TodoFromIntermediateFormat_CategoryIsCorrectlyMapped()
    {
        ((TestSettingsProvider)TestConverter.SettingsProvider).SetIntermediateFormatSymbol(KeyWordType.Folder, "+");
        ((TestSettingsProvider)TestConverter.SettingsProvider).SetIntermediateFormatSymbol(KeyWordType.Context, "@");
        ((TestSettingsProvider)TestConverter.SettingsProvider).SetIntermediateFormatSymbol(KeyWordType.Status, "#");
        ((TestSettingsProvider)TestConverter.SettingsProvider).SetGTDFormatSymbol(KeyWordType.Folder, "");
        ((TestSettingsProvider)TestConverter.SettingsProvider).SetGTDFormatSymbol(KeyWordType.Context, "@");
        ((TestSettingsProvider)TestConverter.SettingsProvider).SetGTDFormatSymbol(KeyWordType.Status, "#");

        var todo = Create.A.Todo().AddCategory("@home").AddCategory("+Ideen").AddCategory("Bestellen").Build();
        var calendar = Create.A.Calendar().WithTask(todo).Build();

        var gtdDataModel = TestConverter.MapFromIntermediateFormat(calendar);
        Assert.Single(gtdDataModel.Tag!);
        Assert.Single(gtdDataModel.Folder!);
        Assert.Single(gtdDataModel.Context!);

        Assert.Equal("Bestellen", gtdDataModel.Tag?.First().Title);
        Assert.Equal("Ideen", gtdDataModel.Folder?.First().Title);
        Assert.Equal("@home", gtdDataModel.Context?.First().Title);
    }

    [Theory]
    [InlineData("#NextAction", TodoBuilder.StatusEnum.InProcess, true, Status.NextAction)]
    [InlineData("#Active", TodoBuilder.StatusEnum.InProcess, true, Status.Active)]
    [InlineData("#Planning", TodoBuilder.StatusEnum.InProcess, true, Status.Planning)]
    [InlineData("#Delegated", TodoBuilder.StatusEnum.InProcess, true, Status.Delegated)]
    [InlineData("#Waiting", TodoBuilder.StatusEnum.InProcess, true, Status.Waiting)]
    [InlineData("#Hold", TodoBuilder.StatusEnum.InProcess, true, Status.Hold)]
    [InlineData("#Postponed", TodoBuilder.StatusEnum.InProcess, true, Status.Postponed)]
    [InlineData("#Someday", TodoBuilder.StatusEnum.InProcess, true, Status.Someday)]
    [InlineData("#Reference", TodoBuilder.StatusEnum.InProcess, true, Status.Reference)]
    [InlineData("#Hello", TodoBuilder.StatusEnum.InProcess, false, Status.Active)]
    [InlineData("#Hello", TodoBuilder.StatusEnum.NeedsAction, false, Status.Waiting)]
    [InlineData("#Hello", TodoBuilder.StatusEnum.Cancelled, false, Status.Canceled)]
    public void Map_TodoFromIntermediateFormat_StatusIsCorrectlyMappedFromStatusTag(string statusTag, TodoBuilder.StatusEnum todoStatus, bool isKnownState, Status expectedStatus)
    {
        ((TestSettingsProvider)TestConverter.SettingsProvider).SetIntermediateFormatSymbol(KeyWordType.Folder, "+");
        ((TestSettingsProvider)TestConverter.SettingsProvider).SetIntermediateFormatSymbol(KeyWordType.Context, "@");
        ((TestSettingsProvider)TestConverter.SettingsProvider).SetIntermediateFormatSymbol(KeyWordType.Status, "#");
        ((TestSettingsProvider)TestConverter.SettingsProvider).SetGTDFormatSymbol(KeyWordType.Folder, "");
        ((TestSettingsProvider)TestConverter.SettingsProvider).SetGTDFormatSymbol(KeyWordType.Context, "@");
        ((TestSettingsProvider)TestConverter.SettingsProvider).SetGTDFormatSymbol(KeyWordType.Status, "#");

        var todo = Create.A.Todo().AddCategory(statusTag).WithStatus(todoStatus).Build();
        var calendar = Create.A.Calendar().WithTask(todo).Build();

        var gtdDataModel = TestConverter.MapFromIntermediateFormat(calendar);

        Assert.Equal(expectedStatus, gtdDataModel.Task?.First().Status);
        if (isKnownState)
        {
            Assert.False(gtdDataModel.Tag?.Any(t => t.Title == statusTag));
        }
        else
        {
            var currentTag = gtdDataModel.Tag!.Single(t => t.Title == statusTag);
            Assert.NotNull(currentTag);
            Assert.True(gtdDataModel.Task?.First().Tag.Contains(currentTag.Id));
        }
    }

    private static void AssertBasicTaskProperties(GTDTaskModel gtdTaskModel, Todo taskAppTaskModel, GTDTaskModel gtdRemappedTaskModel)
    {
        Assert.Equal(gtdTaskModel.Id.ToString(), taskAppTaskModel.Uid);
        Assert.IsType<Calendar>(taskAppTaskModel.Parent);
        Assert.Equal(gtdTaskModel.Title, taskAppTaskModel.Summary);
        var expectedStatus = gtdTaskModel.MapStatus(gtdTaskModel.Completed != null);
        Assert.Equal(expectedStatus, taskAppTaskModel.Status);
        Assert.True(bool.TryParse(taskAppTaskModel.Properties.Get<string>(IntermediateFormatPropertyNames.Starred), out var starred));
        Assert.Equal(gtdTaskModel.Starred, starred);
        Assert.Equal(0, taskAppTaskModel.Priority);
        Assert.Equal(gtdTaskModel.Note, taskAppTaskModel.Description?.GetStringArray());
        Assert.True(bool.TryParse(taskAppTaskModel.Properties.Get<string>(IntermediateFormatPropertyNames.DueFloat), out var floating));
        Assert.Equal(gtdTaskModel.Floating, floating);
        Assert.Equivalent(gtdTaskModel, gtdRemappedTaskModel);
    }

    private void AssertTaskDates(GTDTaskModel gtdTaskModel, Todo taskAppTaskModel)
    {
        Assert.Equal(gtdTaskModel.Created, taskAppTaskModel.Created!.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(gtdTaskModel.Modified, taskAppTaskModel.LastModified!.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(gtdTaskModel.DueDate, taskAppTaskModel.Due!.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(gtdTaskModel.Completed, taskAppTaskModel.Completed!.GetLocalDateTime(CurrentDateTimeZone));
        var hideUntil = taskAppTaskModel.Properties.Get<CalDateTime>(IntermediateFormatPropertyNames.HideUntil);
        var hideUntilMilliseconds = new DateTimeOffset(hideUntil?.Value ?? new DateTime()).ToUnixTimeMilliseconds();
        Assert.Equal(gtdTaskModel.HideUntil, hideUntilMilliseconds);
    }

    private void AssertTaskKeywords(GTDTaskModel gtdTaskModel, Calendar taskAppDataModel)
    {
        var keyWordMetaDataList = keyWordMapperService.GetKeyWordMetaDataIntermediateFormatDictionary(taskAppDataModel!, settingsProvider).Values;
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
            ReminderTestCase.OverLongReminder => 44000,
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

    private static CalDateTime? GetExpectedStartDate(bool? repeatFromDueDate, Todo? taskAppTaskModel)
    {
        if (repeatFromDueDate == null)
            return taskAppTaskModel?.Start;

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
