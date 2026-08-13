using NodaTime;
using TaskConverter.Plugin.Base;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Tests.Utils;

namespace TaskConverter.Plugin.GTD.Tests.MappingTests;

/// <summary>
/// X-DGT-HIDE-UNTIL and X-DGT-START hold a date. In memory the property value is a CalDateTime, but
/// once the calendar has been through text it is a plain string, because ical.net has no way of
/// knowing that a custom property is a date. Reading it as CalDateTime therefore silently yielded
/// null and the hide information was dropped.
/// </summary>
public class HideMappingTests(IConversionService<GTDDataModel> testConverter, IClock clock) : BaseMappingTests(testConverter, clock)
{
    [Fact]
    public void MapThroughText_KeepsHideUntil()
    {
        const long hideUntil = 1677402000000;
        var gtdDataModel = CreateTask(task =>
        {
            task.Hide = Hide.GivenDate;
            task.HideUntil = hideUntil;
        });

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        var task = Assert.Single(remapped!.Task!);
        Assert.Equal(hideUntil, task.HideUntil);
        Assert.Equal(Hide.GivenDate, task.Hide);
    }

    [Fact]
    public void MapThroughText_KeepsHideSixMonthsBeforeDue()
    {
        var dueDate = new LocalDateTime(2023, 8, 23, 0, 0, 0);
        var gtdDataModel = CreateTask(task =>
        {
            task.DueDate = dueDate;
            task.Hide = Hide.SixMonthsBeforeDue;
            task.HideUntil = HideUntilMilliseconds(dueDate.PlusMonths(-6));
        });

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        Assert.Equal(Hide.SixMonthsBeforeDue, Assert.Single(remapped!.Task!).Hide);
    }

    [Fact]
    public void MapThroughText_WithoutHideUntil_StaysDontHide()
    {
        var gtdDataModel = CreateTask(task =>
        {
            task.Hide = Hide.DontHide;
            task.HideUntil = 0;
        });

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        var task = Assert.Single(remapped!.Task!);
        Assert.Equal(Hide.DontHide, task.Hide);
        Assert.Equal(0, task.HideUntil);
    }

    private long HideUntilMilliseconds(LocalDateTime localDateTime) =>
        localDateTime.InZoneLeniently(CurrentDateTimeZone).ToInstant().ToUnixTimeMilliseconds();

    private static GTDDataModel CreateTask(Action<GTDTaskModel> configure)
    {
        var gtdDataModel = Create.A.GTDDataModel().AddTaskList(() => [Create.A.GTDTaskModel(TestConstants.DefaultTaskId).Build()]).Build();
        configure(gtdDataModel.Task!.Single());
        return gtdDataModel;
    }
}
