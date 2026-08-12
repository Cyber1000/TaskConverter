using NodaTime;
using TaskConverter.Plugin.Base;
using TaskConverter.Plugin.GTD.Conversion;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Tests.Utils;
using Period = TaskConverter.Plugin.GTD.Model.Period;

namespace TaskConverter.Plugin.GTD.Tests.MappingTests;

/// <summary>
/// Hide, DueDateModifier and DueTimeSet used to be derived on the way back rather than carried:
/// Hide from comparing the due date against the hide date, DueDateModifier from Floating, DueTimeSet
/// from the due date having a time other than midnight. Real data disagrees with all three, so the
/// value is carried now. The derivation stays as a fallback for calendars from foreign clients,
/// which have no X-DGT properties at all.
/// </summary>
public class DerivedFieldMappingTests(IConversionService<GTDDataModel> testConverter, IClock clock) : BaseMappingTests(testConverter, clock)
{
    [Fact]
    public void MapThroughText_KeepsHideWithoutDueDate()
    {
        var gtdDataModel = CreateTask(task =>
        {
            task.DueDate = null;
            task.Hide = Hide.SixMonthsBeforeDue;
            task.HideUntil = 0;
        });

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        Assert.Equal(Hide.SixMonthsBeforeDue, Assert.Single(remapped!.Task!).Hide);
    }

    [Fact]
    public void MapThroughText_KeepsDueDateModifierOfFloatingTask()
    {
        var gtdDataModel = CreateTask(task =>
        {
            task.Floating = true;
            task.DueDateModifier = DueDateModifier.DueBy;
        });

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        var task = Assert.Single(remapped!.Task!);
        Assert.True(task.Floating);
        Assert.Equal(DueDateModifier.DueBy, task.DueDateModifier);
    }

    [Fact]
    public void MapThroughText_KeepsDueDateModifierOfNonFloatingTask()
    {
        var gtdDataModel = CreateTask(task =>
        {
            task.Floating = false;
            task.DueDateModifier = DueDateModifier.OptionallyOn;
        });

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        var task = Assert.Single(remapped!.Task!);
        Assert.False(task.Floating);
        Assert.Equal(DueDateModifier.OptionallyOn, task.DueDateModifier);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void MapThroughText_KeepsDueTimeSetIndependentOfTheTime(bool dueTimeSet)
    {
        var gtdDataModel = CreateTask(task =>
        {
            task.DueDate = new LocalDateTime(2023, 2, 23, 14, 30, 0);
            task.DueTimeSet = dueTimeSet;
        });

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        Assert.Equal(dueTimeSet, Assert.Single(remapped!.Task!).DueTimeSet);
    }

    /// <summary>
    /// Without the carried values the old derivation has to take over, otherwise a calendar from a
    /// foreign client would end up with nothing at all.
    /// </summary>
    [Fact]
    public void MapFromIntermediateFormat_WithoutCarriedValues_FallsBackToDerivation()
    {
        var gtdDataModel = CreateTask(task =>
        {
            task.DueDate = new LocalDateTime(2023, 2, 23, 14, 30, 0);
            task.DueTimeSet = true;
        });
        var intermediateModel = TestConverter.MapToIntermediateFormat(gtdDataModel);
        var todo = intermediateModel.Todos.Single();
        foreach (var propertyName in new[] { IntermediateFormatPropertyNames.DueTimeSet, IntermediateFormatPropertyNames.DueDateModifier, IntermediateFormatPropertyNames.Hide })
            todo.Properties.Remove(propertyName);

        var remapped = TestConverter.MapFromIntermediateFormat(SerializeAndReparse(intermediateModel));

        Assert.True(Assert.Single(remapped.Task!).DueTimeSet);
    }

    /// <summary>
    /// RFC 5545 needs DTSTART as the base of a recurrence, so a repeating task gets one even when it
    /// has no start date of its own. The way back must not mistake that for a start date: in the
    /// real backup all 5998 tasks have an empty START_DATE, and 42 came back with one.
    /// </summary>
    [Fact]
    public void MapThroughText_RepeatingTaskWithoutStartDate_StaysWithoutStartDate()
    {
        var gtdDataModel = CreateTask(task =>
        {
            task.StartDate = null;
            task.RepeatNew = new GTDRepeatInfoModel(1, Period.Week);
            task.RepeatFrom = GTDRepeatFrom.FromDueDate;
        });

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        Assert.Null(Assert.Single(remapped!.Task!).StartDate);
    }

    [Fact]
    public void MapThroughText_KeepsStartDate()
    {
        var startDate = new LocalDateTime(2023, 2, 26, 10, 0, 0);
        var gtdDataModel = CreateTask(task => task.StartDate = startDate);

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        Assert.Equal(startDate, Assert.Single(remapped!.Task!).StartDate);
    }

    [Fact]
    public void MapThroughText_KeepsDueDateProject()
    {
        var dueDateProject = new LocalDateTime(2023, 1, 1, 0, 0, 0);
        var gtdDataModel = CreateTask(task => task.DueDateProject = dueDateProject);

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        Assert.Equal(dueDateProject, Assert.Single(remapped!.Task!).DueDateProject);
    }

    /// <summary>
    /// RepeatFrom was derived from comparing start and due date. In the backup 192 tasks carry a
    /// RepeatFrom without repeating at all, so there is nothing to derive it from.
    /// </summary>
    [Theory]
    [InlineData(GTDRepeatFrom.FromDueDate, true)]
    [InlineData(GTDRepeatFrom.FromCompletion, true)]
    [InlineData(GTDRepeatFrom.FromCompletion, false)]
    public void MapThroughText_KeepsRepeatFrom(GTDRepeatFrom repeatFrom, bool hasRepetition)
    {
        var gtdDataModel = CreateTask(task =>
        {
            task.RepeatNew = hasRepetition ? new GTDRepeatInfoModel(1, Period.Week) : null;
            task.RepeatFrom = repeatFrom;
        });

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        Assert.Equal(repeatFrom, Assert.Single(remapped!.Task!).RepeatFrom);
    }

    /// <summary>
    /// Alarm and Reminder are two separate GTD fields that both end up in the single VALARM of a
    /// VTODO, so Alarm cannot be reconstructed from the trigger: an absolute reminder produced an
    /// Alarm that was never there (7 tasks), and a due date based reminder dropped the Alarm that
    /// was (11 tasks).
    /// </summary>
    [Fact]
    public void MapThroughText_KeepsAlarmSetAlongsideDueDateBasedReminder()
    {
        var alarm = new LocalDateTime(2019, 3, 1, 9, 15, 0);
        var gtdDataModel = CreateTask(task =>
        {
            task.Alarm = alarm;
            task.Reminder = 0;
        });

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        var task = Assert.Single(remapped!.Task!);
        Assert.Equal(alarm, task.Alarm);
        Assert.Equal(0, task.Reminder);
    }

    [Fact]
    public void MapThroughText_AbsoluteReminderWithoutAlarm_DoesNotInventOne()
    {
        var gtdDataModel = CreateTask(task =>
        {
            task.Alarm = null;
            task.Reminder = 1510642800000;
        });

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        var task = Assert.Single(remapped!.Task!);
        Assert.Null(task.Alarm);
        Assert.Equal(1510642800000, task.Reminder);
    }

    private static GTDDataModel CreateTask(Action<GTDTaskModel> configure)
    {
        var gtdDataModel = Create.A.GTDDataModel().AddTaskList(() => [Create.A.GTDTaskModel(TestConstants.DefaultTaskId).Build()]).Build();
        configure(gtdDataModel.Task!.Single());
        return gtdDataModel;
    }
}
