using NodaTime;
using TaskConverter.Plugin.Base;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Tests.Utils;

namespace TaskConverter.Plugin.GTD.Tests.MappingTests;

/// <summary>
/// Two things RFC 5545 cannot carry: sub second precision on DATE-TIME values, and the GTD task
/// type. Both are transported in X-DGT properties, because the intermediate format is a lossless
/// transport between plugins rather than a calendar meant to look pretty in a foreign client.
/// </summary>
public class PrecisionMappingTests(IConversionService<GTDDataModel> testConverter, IClock clock) : BaseMappingTests(testConverter, clock)
{
    [Theory]
    [InlineData(TaskType.Task)]
    [InlineData(TaskType.Project)]
    [InlineData(TaskType.Checklist)]
    public void MapThroughText_KeepsTaskType(TaskType taskType)
    {
        var gtdDataModel = CreateTask(task => task.Type = taskType);

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        Assert.Equal(taskType, Assert.Single(remapped!.Task!).Type);
    }

    [Fact]
    public void MapThroughText_KeepsMillisecondsOfCreatedAndModified()
    {
        var created = new LocalDateTime(2018, 1, 26, 0, 18, 20, 288);
        var modified = new LocalDateTime(2021, 9, 26, 13, 38, 52, 732);
        var gtdDataModel = CreateTask(task =>
        {
            task.Created = created;
            task.Modified = modified;
        });

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        var task = Assert.Single(remapped!.Task!);
        Assert.Equal(created, task.Created);
        Assert.Equal(modified, task.Modified);
    }

    [Fact]
    public void MapThroughText_KeepsMillisecondsOfCompleted()
    {
        var completed = new LocalDateTime(2019, 2, 15, 13, 8, 56, 349);
        var gtdDataModel = CreateTask(task => task.Completed = completed);

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        Assert.Equal(completed, Assert.Single(remapped!.Task!).Completed);
    }

    /// <summary>
    /// A calendar written by a foreign client has no X-DGT properties at all, so the mapping has to
    /// fall back to plain second precision instead of failing.
    /// </summary>
    [Fact]
    public void MapFromIntermediateFormat_WithoutPrecisionProperties_FallsBackToSeconds()
    {
        var created = new LocalDateTime(2018, 1, 26, 0, 18, 20, 288);
        var gtdDataModel = CreateTask(task => task.Created = created);
        var intermediateModel = TestConverter.MapToIntermediateFormat(gtdDataModel);
        var todo = intermediateModel.Todos.Single();
        foreach (var property in todo.Properties.Where(p => p.Name.StartsWith("X-DGT-", StringComparison.OrdinalIgnoreCase)).ToList())
            todo.Properties.Remove(property.Name);

        var remapped = TestConverter.MapFromIntermediateFormat(SerializeAndReparse(intermediateModel));

        Assert.Equal(created.PlusNanoseconds(-created.NanosecondOfSecond), Assert.Single(remapped.Task!).Created);
    }

    private static GTDDataModel CreateTask(Action<GTDTaskModel> configure)
    {
        var gtdDataModel = Create.A.GTDDataModel().AddTaskList(() => [Create.A.GTDTaskModel(TestConstants.DefaultTaskId).Build()]).Build();
        configure(gtdDataModel.Task!.Single());
        return gtdDataModel;
    }
}
