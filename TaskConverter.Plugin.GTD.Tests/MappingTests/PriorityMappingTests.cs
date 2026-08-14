using NodaTime;
using TaskConverter.Plugin.Base;
using TaskConverter.Plugin.GTD.Conversion;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Tests.Utils;

namespace TaskConverter.Plugin.GTD.Tests.MappingTests;

/// <summary>
/// GTD's Low is the default of its enum, so it means "never touched" rather than "low" - 5144 of
/// 5998 tasks in a real backup carry it. Mapping that onto RFC 5545 priority 7 made every one of
/// them show up as an explicitly low priority task in the target app. Low and None therefore both
/// become 0 ("no priority"), and the exact GTD value travels in X-DGT-PRIORITY so nothing is lost.
/// </summary>
public class PriorityMappingTests(IConversionService<GTDDataModel> testConverter, IClock clock) : BaseMappingTests(testConverter, clock)
{
    [Theory]
    [InlineData(Priority.Top)]
    [InlineData(Priority.High)]
    [InlineData(Priority.Med)]
    [InlineData(Priority.Low)]
    [InlineData(Priority.None)]
    public void MapThroughText_KeepsPriority(Priority priority)
    {
        var gtdDataModel = CreateWithPriority(priority);

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        Assert.Equal(priority, Assert.Single(remapped!.Task!).Priority);
    }

    [Theory]
    [InlineData(Priority.Top, 1)]
    [InlineData(Priority.High, 3)]
    [InlineData(Priority.Med, 5)]
    [InlineData(Priority.Low, 0)]
    [InlineData(Priority.None, 0)]
    public void MapToIntermediateFormat_WritesIcalPriority(Priority priority, int expectedIcalPriority)
    {
        var gtdDataModel = CreateWithPriority(priority);

        var (intermediateModel, _) = GetMappedInfo(gtdDataModel);

        Assert.Equal(expectedIcalPriority, Assert.Single(intermediateModel!.Todos).Priority);
    }

    /// <summary>
    /// Low and None are indistinguishable in the standard property, so the exact value has to be
    /// carried - otherwise the roundtrip, which is how this converter is measured, would lose it.
    /// </summary>
    [Theory]
    [InlineData(Priority.Low)]
    [InlineData(Priority.None)]
    public void MapToIntermediateFormat_CarriesTheExactPriority(Priority priority)
    {
        var gtdDataModel = CreateWithPriority(priority);

        var (intermediateModel, _) = GetMappedInfo(gtdDataModel);

        Assert.Equal(priority.ToString(), Assert.Single(intermediateModel!.Todos).Properties.Get<string>(IntermediateFormatPropertyNames.Priority));
    }

    /// <summary>
    /// A calendar written by a foreign client carries no X-DGT-PRIORITY, so the full 0 to 9 range
    /// has to map onto something sensible - and 0 means "undefined" there, not "low".
    /// </summary>
    [Theory]
    [InlineData(0, Priority.None)]
    [InlineData(1, Priority.Top)]
    [InlineData(2, Priority.Top)]
    [InlineData(3, Priority.High)]
    [InlineData(4, Priority.High)]
    [InlineData(5, Priority.Med)]
    [InlineData(6, Priority.Low)]
    [InlineData(9, Priority.Low)]
    public void MapFromIntermediateFormat_WithoutCarriedValue_UsesTheFullIcalRange(int icalPriority, Priority expected)
    {
        var gtdDataModel = CreateWithPriority(Priority.None);
        var intermediateModel = TestConverter.MapToIntermediateFormat(gtdDataModel);
        var todo = intermediateModel.Todos.Single();
        todo.Properties.Remove(IntermediateFormatPropertyNames.Priority);
        todo.Priority = icalPriority;

        var remapped = TestConverter.MapFromIntermediateFormat(SerializeAndReparse(intermediateModel));

        Assert.Equal(expected, Assert.Single(remapped.Task!).Priority);
    }

    private static GTDDataModel CreateWithPriority(Priority priority)
    {
        var gtdDataModel = Create.A.GTDDataModel().AddTaskList(() => [Create.A.GTDTaskModel(TestConstants.DefaultTaskId).Build()]).Build();
        gtdDataModel.Task!.Single().Priority = priority;
        return gtdDataModel;
    }
}
