using NodaTime;
using TaskConverter.Plugin.Base;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Tests.Utils;

namespace TaskConverter.Plugin.GTD.Tests.MappingTests;

/// <summary>
/// The writer emits a status category for every status except None and Canceled. A missing
/// category therefore carries information and must not be replaced by a guess.
/// </summary>
public class StatusMappingTests(IConversionService<GTDDataModel> testConverter, IClock clock) : BaseMappingTests(testConverter, clock)
{
    [Theory]
    [InlineData(Status.None)]
    [InlineData(Status.NextAction)]
    [InlineData(Status.Active)]
    [InlineData(Status.Planning)]
    [InlineData(Status.Delegated)]
    [InlineData(Status.Waiting)]
    [InlineData(Status.Hold)]
    [InlineData(Status.Postponed)]
    [InlineData(Status.Someday)]
    [InlineData(Status.Canceled)]
    [InlineData(Status.Reference)]
    public void MapThroughText_KeepsStatus(Status status)
    {
        var gtdDataModel = CreateWithStatus(status, isCompleted: false);

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        Assert.Equal(status, Assert.Single(remapped!.Task!).Status);
    }

    /// <summary>
    /// A completed task carries STATUS:COMPLETED, which used to override the status entirely:
    /// every completed task without a status category came back as Active.
    /// </summary>
    [Theory]
    [InlineData(Status.None)]
    [InlineData(Status.Active)]
    [InlineData(Status.Someday)]
    public void MapThroughText_KeepsStatusOfCompletedTask(Status status)
    {
        var gtdDataModel = CreateWithStatus(status, isCompleted: true);

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        Assert.Equal(status, Assert.Single(remapped!.Task!).Status);
    }

    private static GTDDataModel CreateWithStatus(Status status, bool isCompleted)
    {
        var builder = Create.A.GTDTaskModel(TestConstants.DefaultTaskId);
        if (!isCompleted)
            builder = builder.WithCompletedDate(null);

        var gtdDataModel = Create.A.GTDDataModel().AddTaskList(() => [builder.Build()]).Build();
        gtdDataModel.Task!.Single().Status = status;
        return gtdDataModel;
    }
}
