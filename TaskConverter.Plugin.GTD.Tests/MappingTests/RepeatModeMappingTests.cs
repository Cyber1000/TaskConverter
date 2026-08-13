using Ical.Net.DataTypes;
using Ical.Net;
using NodaTime;
using TaskConverter.Plugin.Base;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Tests.Utils;

namespace TaskConverter.Plugin.GTD.Tests.MappingTests;

/// <summary>
/// The app offers sixteen repeat modes. Most decompose into an interval and a period, but
/// BusinessDay and Weekend are weekday sets - which RFC 5545 expresses exactly, as BYDAY.
/// The remaining four have no iCalendar equivalent and must be refused with a message that
/// names the mode instead of an exception meaning "the developer has not got round to it".
/// </summary>
public class RepeatModeMappingTests(IConversionService<GTDDataModel> testConverter, IClock clock) : BaseMappingTests(testConverter, clock)
{
    [Theory]
    [InlineData("BusinessDay", RepeatPattern.BusinessDay)]
    [InlineData("Business Day", RepeatPattern.BusinessDay)]
    [InlineData("Weekend", RepeatPattern.Weekend)]
    public void Parse_WeekdaySetModes(string repeatInfo, RepeatPattern expected)
    {
        Assert.Equal(expected, new GTDRepeatInfoModel(repeatInfo).Pattern);
    }

    [Theory]
    [InlineData(RepeatPattern.BusinessDay, new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday })]
    [InlineData(RepeatPattern.Weekend, new[] { DayOfWeek.Saturday, DayOfWeek.Sunday })]
    public void MapToIntermediateFormat_WeekdaySetModes_BecomeByDay(RepeatPattern pattern, DayOfWeek[] expectedDays)
    {
        var gtdDataModel = CreateTaskWithRepeat(new GTDRepeatInfoModel(pattern));

        var (intermediateModel, _) = GetMappedInfo(gtdDataModel);

        var recurrence = Assert.Single(Assert.Single(intermediateModel!.Todos).RecurrenceRules);
        Assert.Equal(FrequencyType.Weekly, recurrence.Frequency);
        Assert.Equal(expectedDays.Order(), recurrence.ByDay.Select(d => d.DayOfWeek).Order());
    }

    [Theory]
    [InlineData(RepeatPattern.BusinessDay)]
    [InlineData(RepeatPattern.Weekend)]
    public void MapThroughText_KeepsWeekdaySetModes(RepeatPattern pattern)
    {
        var gtdDataModel = CreateTaskWithRepeat(new GTDRepeatInfoModel(pattern));

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        var repeat = Assert.Single(remapped!.Task!).RepeatNew;
        Assert.NotNull(repeat);
        Assert.Equal(pattern, repeat!.Value.Pattern);
    }

    [Theory]
    [InlineData(RepeatPattern.BusinessDay, "BusinessDay")]
    [InlineData(RepeatPattern.Weekend, "Weekend")]
    public void ToString_WeekdaySetModes_UseTheNameTheAppWrites(RepeatPattern pattern, string expected)
    {
        Assert.Equal(expected, new GTDRepeatInfoModel(pattern).ToString());
    }

    /// <summary>
    /// These four have no counterpart in RFC 5545 and are refused. The message has to name the
    /// value, because the whole point is that the user learns which of their tasks is affected.
    /// </summary>
    [Theory]
    [InlineData("With parent")]
    [InlineData("Every Monday")]
    [InlineData("The 2nd Monday of each month")]
    [InlineData("Last day of every 3 months")]
    public void Parse_UnsupportedModes_AreRefusedWithTheirName(string repeatInfo)
    {
        var exception = Assert.Throws<UnsupportedRepeatModeException>(() => new GTDRepeatInfoModel(repeatInfo));

        Assert.Contains(repeatInfo, exception.Message);
    }

    private static GTDDataModel CreateTaskWithRepeat(GTDRepeatInfoModel repeat)
    {
        var gtdDataModel = Create.A.GTDDataModel().AddTaskList(() => [Create.A.GTDTaskModel(TestConstants.DefaultTaskId).Build()]).Build();
        gtdDataModel.Task!.Single().RepeatNew = repeat;
        return gtdDataModel;
    }
}
