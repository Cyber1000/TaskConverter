using NodaTime;
using TaskConverter.Plugin.Base;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Tests.Utils;

namespace TaskConverter.Plugin.GTD.Tests.MappingTests;

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
    [InlineData(Priority.Low, 7)]
    [InlineData(Priority.None, 0)]
    public void MapToIntermediateFormat_WritesIcalPriority(Priority priority, int expectedIcalPriority)
    {
        var gtdDataModel = CreateWithPriority(priority);

        var (intermediateModel, _) = GetMappedInfo(gtdDataModel);

        Assert.Equal(expectedIcalPriority, Assert.Single(intermediateModel!.Todos).Priority);
    }

    /// <summary>
    /// A foreign CalDAV client may use any value from 0 to 9, so the reverse mapping has to cover
    /// the full range rather than casting the number onto the enum.
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
    public void MapFromIntermediateFormat_AcceptsFullIcalRange(int icalPriority, Priority expected)
    {
        var gtdDataModel = CreateWithPriority(Priority.None);
        var intermediateModel = TestConverter.MapToIntermediateFormat(gtdDataModel);
        intermediateModel.Todos.Single().Priority = icalPriority;

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
