using NodaTime;
using TaskConverter.Plugin.Base;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Tests.Utils;

namespace TaskConverter.Plugin.GTD.Tests.MappingTests;

/// <summary>
/// GTD ids are only unique per entity type, so using them as UID unchanged made a notebook and a
/// task collide. That is not visible inside a single calendar, but IcalWriter names its files after
/// the UID and silently overwrote one with the other.
/// </summary>
public class UidMappingTests(IConversionService<GTDDataModel> testConverter, IClock clock) : BaseMappingTests(testConverter, clock)
{
    private const int SharedId = 5;

    [Fact]
    public void MapToIntermediateFormat_TaskAndNotebookWithSameId_GetDifferentUids()
    {
        var gtdDataModel = Create
            .A.GTDDataModel()
            .AddNotebook(SharedId, folderId: 0)
            .AddTaskList(() => [Create.A.GTDTaskModel(SharedId).Build()])
            .Build();

        var (intermediateModel, _) = GetMappedInfo(gtdDataModel);

        var todoUid = Assert.Single(intermediateModel!.Todos).Uid;
        var journalUid = Assert.Single(intermediateModel.Journals).Uid;
        Assert.NotEqual(todoUid, journalUid);
        Assert.Equal("task-5", todoUid);
        Assert.Equal("notebook-5", journalUid);
    }

    [Fact]
    public void MapThroughText_TaskAndNotebookWithSameId_KeepBothIds()
    {
        var gtdDataModel = Create
            .A.GTDDataModel()
            .AddNotebook(SharedId, folderId: 0)
            .AddTaskList(() => [Create.A.GTDTaskModel(SharedId).Build()])
            .Build();

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        Assert.Equal(SharedId, Assert.Single(remapped!.Task!).Id);
        Assert.Equal(SharedId, Assert.Single(remapped.Notebook!).Id);
    }

    [Fact]
    public void MapThroughText_KeepsParentReference()
    {
        var gtdDataModel = Create
            .A.GTDDataModel()
            .AddTaskList(() =>
                [
                    Create.A.GTDTaskModel(TestConstants.DefaultTaskId).Build(),
                    Create.A.GTDTaskModel(TestConstants.DefaultTaskId + 1).WithParent(TestConstants.DefaultTaskId).Build(),
                ]
            )
            .Build();

        var (_, remapped) = GetMappedInfoThroughText(gtdDataModel);

        var child = remapped!.Task!.Single(t => t.Id == TestConstants.DefaultTaskId + 1);
        Assert.Equal(TestConstants.DefaultTaskId, child.Parent);
    }

    /// <summary>
    /// A calendar from a foreign client has an arbitrary UID, which must still yield a usable id.
    /// </summary>
    [Fact]
    public void MapFromIntermediateFormat_ForeignUid_DoesNotThrow()
    {
        var gtdDataModel = Create.A.GTDDataModel().AddTaskList(() => [Create.A.GTDTaskModel(TestConstants.DefaultTaskId).Build()]).Build();
        var intermediateModel = TestConverter.MapToIntermediateFormat(gtdDataModel);
        intermediateModel.Todos.Single().Uid = "3f8a1c2e-some-foreign-uid";

        var remapped = TestConverter.MapFromIntermediateFormat(SerializeAndReparse(intermediateModel));

        Assert.NotEqual(0, Assert.Single(remapped.Task!).Id);
    }
}
