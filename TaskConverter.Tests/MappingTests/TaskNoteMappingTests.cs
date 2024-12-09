using NodaTime;
using TaskConverter.Model.Mapper;
using TaskConverter.Plugin.GTD.Mapper;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Tests.Utils;

namespace TaskConverter.Tests.MappingTests;

//TODO HH: simplify (use asserts between objects, maybe use a general model for mapping)
public class TaskNoteMappingTests(IConverter testConverter, IClock clock, IConverterDateTimeZoneProvider converterDateTimeZoneProvider) : BaseMappingTests(testConverter, clock, converterDateTimeZoneProvider)
{
    [Fact]
    public void Map_TaskNote()
    {
        var gtdDataModel = CreateGTDDataModelWithTaskNote();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var gtdTaskNoteModel = gtdDataModel.TaskNote![0];
        var taskAppTaskNoteModel = taskAppDataModel?.TaskNotes?[0]!;
        var gtdRemappedTaskNoteModel = gtdDataMappedRemappedModel?.TaskNote?[0]!;

        AssertMappedModelEquivalence(gtdTaskNoteModel, taskAppTaskNoteModel, gtdRemappedTaskNoteModel);
    }

    private static GTDDataModel CreateGTDDataModelWithTaskNote()
    {
        return Create.A.GTDDataModel().AddTaskNote(TestConstants.DefaultTaskNoteId).Build();
    }
}
