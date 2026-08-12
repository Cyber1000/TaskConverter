using NodaTime;
using TaskConverter.Plugin.Base;
using TaskConverter.Plugin.GTD.Conversion;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Tests.Utils;

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

    private static GTDDataModel CreateTask(Action<GTDTaskModel> configure)
    {
        var gtdDataModel = Create.A.GTDDataModel().AddTaskList(() => [Create.A.GTDTaskModel(TestConstants.DefaultTaskId).Build()]).Build();
        configure(gtdDataModel.Task!.Single());
        return gtdDataModel;
    }
}
