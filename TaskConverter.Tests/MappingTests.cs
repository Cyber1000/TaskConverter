using System.Drawing;
using NodaTime;
using TaskConverter.Model.Mapper;
using TaskConverter.Model.Model;
using TaskConverter.Plugin.GTD.Mapper;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Tests.Extensions;
using TaskConverter.Tests.TestData;

namespace TaskConverter.Tests;

//TODO HH: simplify (use asserts between objects, maybe use a general model for mapping)
public class MappingTests(IConverter TestConverter, IClock clock, IConverterDateTimeZoneProvider converterDateTimeZoneProvider)
{
    private DateTimeZone CurrentDateTimeZone => TestConverter.DateTimeZoneProvider.CurrentDateTimeZone;

    [Fact]
    public void Automapper_CheckConfig_ShouldBeValid()
    {
        var gtdMapper = new GTDMapper(clock, converterDateTimeZoneProvider);
        gtdMapper.Mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_Version_ShouldBeValid()
    {
        var gtdDataModel = Create.A.GTDDataModel().Build();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var version = gtdDataModel.Version;
        var versionModel = taskAppDataModel?.Version;
        var versionFromModel = gtdDataMappedRemappedModel?.Version;

        Assert.Equal(version, versionModel);

        Assert.Equal(version, versionFromModel);
    }

    [Fact]
    public void Map_Folder_ShouldBeValid()
    {
        var gtdDataModel = Create.A.GTDDataModel().AddDefaultFolder(1).Build();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var folder = gtdDataModel.Folder![0];
        var folderModel = taskAppDataModel!.KeyWords!.First(t => t.KeyWordType == GTDKeyWordEnum.Folder);
        var folderFromModel = gtdDataMappedRemappedModel?.Folder?[0]!;

        Assert.Equal(folder.Id.ToString(), folderModel.Id);
        Assert.Equal(folder.Created, folderModel.Created.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(folder.Modified, folderModel.Modified.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(folder.Title, folderModel.Title);
        Assert.Equal(Color.FromArgb(folder.Color), folderModel.Color);
        Assert.Equal(folder.Visible, folderModel.Visible);

        Assert.Equivalent(folder, folderFromModel);
    }

    [Fact]
    public void Map_Context_ShouldBeValid()
    {
        var gtdDataModel = Create.A.GTDDataModel().AddDefaultContext(1).Build();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var context = gtdDataModel.Context![0];
        var contextModel = taskAppDataModel!.KeyWords!.First(t => t.KeyWordType == GTDKeyWordEnum.Context);
        var contextFromModel = gtdDataMappedRemappedModel?.Context?[0]!;

        Assert.Equal(context.Id.ToString(), contextModel.Id);
        Assert.Equal(context.Created, contextModel.Created.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(context.Modified, contextModel.Modified.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(context.Title, contextModel.Title);
        Assert.Equal(Color.FromArgb(context.Color), contextModel.Color);
        Assert.Equal(context.Visible, contextModel.Visible);

        Assert.Equivalent(context, contextFromModel);
    }

    [Fact]
    public void Map_Tag_ShouldBeValid()
    {
        var gtdDataModel = Create.A.GTDDataModel().AddDefaultTag(1).Build();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var tag = gtdDataModel.Tag![0];
        var tagModel = taskAppDataModel!.KeyWords!.First(t => t.KeyWordType == KeyWordEnum.Tag);
        var tagFromModel = gtdDataMappedRemappedModel?.Tag?[0]!;

        Assert.Equal(tag.Id.ToString(), tagModel.Id);
        Assert.Equal(tag.Created, tagModel.Created.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(tag.Modified, tagModel.Modified.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(tag.Title, tagModel.Title);
        Assert.Equal(Color.FromArgb(tag.Color), tagModel.Color);
        Assert.Equal(tag.Visible, tagModel.Visible);

        Assert.Equivalent(tag, tagFromModel);
    }

    [Fact]
    public void Map_Task_ShouldBeValid()
    {
        var gtdDataModel = Create
            .A.GTDDataModel()
            .AddDefaultFolder(5)
            .AddDefaultContext(6)
            .AddDefaultTag(7)
            .AddDefaultTag(8)
            .AddDefaultTask(1, 5, 6, [7, 8], 10)
            .AddDefaultTask(10, 0, 0, [], 0)
            .Build();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var task = gtdDataModel.Task![0];
        var taskModel = taskAppDataModel?.Tasks?[0]!;
        var taskModelWithoutParent = taskAppDataModel?.Tasks?[1]!;

        var taskFromModel = gtdDataMappedRemappedModel?.Task?[0]!;
        var taskFromModelWithoutParent = gtdDataMappedRemappedModel?.Task?[1]!;

        Assert.Equal(task.Id.ToString(), taskModel.Id);
        Assert.Equal(task.Parent.ToString(), taskModel.Parent!.Id);
        Assert.Equal(task.Created, taskModel.Created.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(task.Modified, taskModel.Modified.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(task.Title, taskModel.Title);
        Assert.Equal(task.DueDate, taskModel.DueDate);
        Assert.Equal(task.DueDateProject, taskModel.DueDateProject);
        Assert.Null(taskModel.Reminder);
        Assert.Equal(task.RepeatNew?.Interval, taskModel.RepeatInfo?.Interval);
        Assert.Equal(task.RepeatNew?.Period, taskModel.RepeatInfo?.Period);
        Assert.Equal(task.RepeatFrom, (GTDRepeatFrom?)taskModel.RepeatInfo?.RepeatFrom ?? GTDRepeatFrom.FromDueDate);
        Assert.Equal(Model.Model.Status.NextAction, taskModel.Status);
        Assert.Equal(task.Context.ToString(), GetFirstIdOfKeyWord(taskModel, GTDKeyWordEnum.Context));
        Assert.Equal(task.Folder.ToString(), GetFirstIdOfKeyWord(taskModel, GTDKeyWordEnum.Folder));
        Assert.Equal(task.Tag.Select(t => t.ToString()), taskModel.KeyWords.Where(t => t.KeyWordType == KeyWordEnum.Tag).Select(t => t.Id));
        Assert.Equal(task.Starred, taskModel.Starred);
        Assert.Equal(Model.Model.Priority.Low, taskModel.Priority);
        Assert.Equal(task.Note, taskModel.Note);
        Assert.Equal(task.Completed, taskModel.Completed);
        Assert.Equal(Model.Model.TaskType.Task, taskModel.Type);
        Assert.Equal(task.Floating, taskModel.HasFloatingDueDate);
        Assert.Equal(task.HideUntil, taskModel.HideUntil!.Value.ToUnixTimeMilliseconds());

        Assert.Null(taskModelWithoutParent.Parent);
        Assert.Equal(0, taskFromModelWithoutParent.Parent);

        Assert.Equivalent(task, taskFromModel);

        string GetFirstIdOfKeyWord(TaskModel taskModel, KeyWordEnum keyWordEnum)
        {
            return taskModel.KeyWords.FirstOrDefault(t => t.KeyWordType == keyWordEnum)!.Id;
        }
    }

    [Fact]
    public void Map_TaskNote_ShouldBeValid()
    {
        var gtdDataModel = Create.A.GTDDataModel().AddDefaultTaskNote(1).Build();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var taskNote = gtdDataModel.TaskNote![0];
        var taskNoteModel = taskAppDataModel?.TaskNotes?[0]!;
        var taskNoteFromModel = gtdDataMappedRemappedModel?.TaskNote?[0]!;

        Assert.Equal(taskNote.Id.ToString(), taskNoteModel.Id);
        Assert.Equal(taskNote.Created, taskNoteModel.Created.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(taskNote.Modified, taskNoteModel.Modified.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(taskNote.Title, taskNoteModel.Title);
        Assert.Equal(Color.FromArgb(taskNote.Color), taskNoteModel.Color);
        Assert.Equal(taskNote.Visible, taskNoteModel.Visible);

        Assert.Equivalent(taskNote, taskNoteFromModel);
    }

    [Fact]
    public void Map_Notebook_ShouldBeValid()
    {
        var gtdDataModel = Create.A.GTDDataModel().AddDefaultFolder(2).AddDefaultNotebook(1).Build();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var notebook = gtdDataModel.Notebook![0];
        var notebookModel = taskAppDataModel?.Notebooks?[0]!;
        var notebookFromModel = gtdDataMappedRemappedModel?.Notebook?[0]!;

        Assert.Equal(notebook.Id.ToString(), notebookModel.Id);
        Assert.Equal(notebook.Created, notebookModel.Created.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(notebook.Modified, notebookModel.Modified.GetLocalDateTime(CurrentDateTimeZone));
        Assert.Equal(notebook.Title, notebookModel.Title);
        Assert.Equal(notebook.Note, notebookModel.Note);
        Assert.Equal(notebook.FolderId.ToString(), notebookModel.Keyword?.Id);
        Assert.Equal(Color.FromArgb(notebook.Color), notebookModel.Color);
        Assert.Equal(notebook.Visible, notebookModel.Visible);

        Assert.Equivalent(notebook, notebookFromModel);
    }

    [Fact]
    public void Map_Preferences_ShouldBeValid()
    {
        var gtdDataModel = Create.A.GTDDataModel().AddPreferences().Build();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);
        var preference = gtdDataModel.Preferences![0].XmlConfig;
        var preferenceModel = taskAppDataModel?.Config;
        var preferenceFromModel = gtdDataMappedRemappedModel?.Preferences?[0]?.XmlConfig;

        Assert.Equal(preferenceModel, preference);

        Assert.Equal(preferenceFromModel, preference);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Map_TaskWithDueDate_ShouldBeValid(bool hasTime)
    {
        var dueDate = hasTime ? new LocalDateTime(2023, 02, 20, 10, 0, 0) : new LocalDateTime(2023, 02, 20, 0, 0, 0);
        var gtdDataModel = Create.A.GTDDataModel().AddDefaultTask(1, 0, 0, [], 0, dueDate).Build();

        var (_, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);

        var taskFromModel = gtdDataMappedRemappedModel?.Task?[0]!;

        Assert.Equal(hasTime, taskFromModel.DueTimeSet);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Map_TaskWithDueDateModifier_ShouldBeValid(bool isFloating)
    {
        var gtdDataModel = Create.A.GTDDataModel().AddDefaultTask(1, 0, 0, [], 0, null, null, isFloating).Build();

        var (_, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);

        var taskFromModel = gtdDataMappedRemappedModel?.Task?[0]!;

        Assert.Equal(isFloating ? DueDateModifier.OptionallyOn : DueDateModifier.DueBy, taskFromModel.DueDateModifier);
    }

    [Theory]
    [InlineData(-1, true, null)]
    [InlineData(0, true, BaseDateOfReminderInstant.FromDueDate)]
    [InlineData(180, true, BaseDateOfReminderInstant.FromDueDate)]
    [InlineData(1080, true, BaseDateOfReminderInstant.FromDueDate)]
    [InlineData(43200, true, BaseDateOfReminderInstant.FromDueDate)]
    [InlineData(1608541200000, true, BaseDateOfReminderInstant.FromUnixEpoch)]
    [InlineData(43201, true, BaseDateOfReminderInstant.FromUnixEpoch)]
    [InlineData(43199, false, BaseDateOfReminderInstant.FromDueDate)]
    [InlineData(1081, false, BaseDateOfReminderInstant.FromDueDate)]
    public void Map_TaskWithReminder_ShouldBeValid(long reminder, bool expectOriginalValue, BaseDateOfReminderInstant? expectedBase)
    {
        var dueDateInstant = Instant.FromUnixTimeMilliseconds(1608541200000);
        var dueDate = dueDateInstant.GetLocalDateTime(CurrentDateTimeZone);
        var gtdDataModel = Create.A.GTDDataModel().AddDefaultTask(1, 0, 0, [], 0, dueDate, null, null, reminder).Build();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);

        var taskModel = taskAppDataModel?.Tasks?[0]!;
        var taskFromModel = gtdDataMappedRemappedModel?.Task?[0]!;

        Assert.Equal(expectedBase, taskModel.Reminder?.ReminderInstantType);
        var expectedReminder = expectOriginalValue
            ? reminder
            : dueDateInstant.Plus(-Duration.FromMinutes(reminder)).ToUnixTimeMilliseconds();
        Assert.Equal(expectedReminder, taskFromModel.Reminder);
    }

    [Fact]
    public void Map_TaskWithReminderWithDueDateBasedReminderWithoutDueDate_ShouldReturnMinusOne()
    {
        var gtdDataModel = Create.A.GTDDataModel().AddTask(1, 0, 0, [], 0, null, null, null, 30000).Build();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);

        var taskModel = taskAppDataModel?.Tasks?[0]!;
        var taskFromModel = gtdDataMappedRemappedModel?.Task?[0]!;

        Assert.Equal(BaseDateOfReminderInstant.FromDueDate, taskModel.Reminder?.ReminderInstantType);
        Assert.Null(taskModel.Reminder?.AbsoluteInstant);
        Assert.Equal(-1, taskFromModel.Reminder);
    }

    [Theory]
    [InlineData(-1, true)]
    [InlineData(0, true)]
    [InlineData(1, false)]
    public void Map_TaskWithAlarm_ShouldBeValid(int addMinutes, bool shouldHaveAlarm)
    {
        var currentDateTime = TestConverter.Clock.GetCurrentInstant();
        var reminder = currentDateTime.Plus(Duration.FromMinutes(addMinutes)).ToUnixTimeMilliseconds();

        var gtdDataModel = Create.A.GTDDataModel().AddDefaultTask(1, 0, 0, [], 0, null, null, null, reminder).Build();
        var (_, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);

        var taskFromModel = gtdDataMappedRemappedModel?.Task?[0]!;

        if (shouldHaveAlarm)
            Assert.Equal(reminder, taskFromModel.Alarm!.Value.InZoneLeniently(CurrentDateTimeZone).ToInstant().ToUnixTimeMilliseconds());
        else
            Assert.Null(taskFromModel.Alarm);
    }

    [Theory]
    [InlineData("Every 1 day", GTDRepeatFrom.FromDueDate, 1, Model.Model.Period.Day, true)]
    [InlineData("Every 2 weeks", GTDRepeatFrom.FromDueDate, 2, Model.Model.Period.Week, true)]
    [InlineData("Every 3 months", GTDRepeatFrom.FromDueDate, 3, Model.Model.Period.Month, true)]
    [InlineData("Every 4 years", GTDRepeatFrom.FromDueDate, 4, Model.Model.Period.Year, true)]
    [InlineData("Every 1 day", GTDRepeatFrom.FromCompletion, 1, Model.Model.Period.Day, true)]
    [InlineData("every 1 day", GTDRepeatFrom.FromDueDate, 1, Model.Model.Period.Day, true)]
    [InlineData(null, GTDRepeatFrom.FromDueDate, 1, Model.Model.Period.Day, false)]
    [InlineData(null, GTDRepeatFrom.FromCompletion, 1, Model.Model.Period.Day, false)]
    public void Map_Repeat_ShouldBeValid(
        string repeatInfoString,
        GTDRepeatFrom repeatFrom,
        int expectedInterval,
        Model.Model.Period expectedPeriod,
        bool expectRepeatInfo
    )
    {
        var repeatNew = expectRepeatInfo ? new GTDRepeatInfoModel(repeatInfoString) : (GTDRepeatInfoModel?)null;
        var gtdDataModel = Create.A.GTDDataModel().AddTask(1, 0, 0, [], 0, null, repeatNew, null, null, repeatFrom).Build();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);

        var taskModel = taskAppDataModel?.Tasks?[0]!;
        var taskFromModel = gtdDataMappedRemappedModel?.Task?[0]!;

        if (expectRepeatInfo)
        {
            var gtdRepeatFrom = (GTDRepeatFrom?)taskModel.RepeatInfo!.Value.RepeatFrom;
            Assert.Equal(expectedInterval, taskModel.RepeatInfo!.Value.Interval);
            Assert.Equal(expectedPeriod, taskModel.RepeatInfo!.Value.Period);
            Assert.Equal(repeatFrom, gtdRepeatFrom);

            Assert.Equal(repeatFrom, taskFromModel.RepeatFrom);
            Assert.Equal(repeatInfoString.ToLowerInvariant(), taskFromModel.RepeatNew!.Value.ToString().ToLowerInvariant());
        }
        else
        {
            Assert.Null(taskModel.RepeatInfo);
            Assert.Equal(RepeatFrom.FromDueDate, (RepeatFrom)taskFromModel.RepeatFrom);
            Assert.Null(taskFromModel.RepeatNew);
        }
    }

    [Theory]
    [InlineData(Hide.SixMonthsBeforeDue)]
    [InlineData(Hide.GivenDate)]
    [InlineData(Hide.DontHide)]
    public void Map_Hide_ShouldBeValid(Hide hide)
    {
        var hideUntil = hide switch
        {
            Hide.SixMonthsBeforeDue => (Instant?)new LocalDateTime(2022, 08, 20, 10, 0, 0).InZoneLeniently(CurrentDateTimeZone).ToInstant(),
            Hide.GivenDate => (Instant?)new LocalDateTime(2022, 05, 10, 10, 0, 0).InZoneLeniently(CurrentDateTimeZone).ToInstant(),
            Hide.DontHide => null,
            _ => throw new NotImplementedException($"HideInfo {hide} not implemented"),
        };
        var hideInMilliseconds = hideUntil.HasValue ? hideUntil.Value.ToUnixTimeMilliseconds() : 0;
        var dueDate = new LocalDateTime(2023, 02, 20, 10, 0, 0);

        var gtdDataModel = Create
            .A.GTDDataModel()
            .AddDefaultTask(1, 0, 0, [], 0, dueDate, null, null, null, null, Hide.SixMonthsBeforeDue, hideInMilliseconds)
            .Build();

        var (taskAppDataModel, gtdDataMappedRemappedModel) = GetMappedInfo(gtdDataModel);

        var taskModel = taskAppDataModel?.Tasks?[0]!;
        var taskFromModel = gtdDataMappedRemappedModel?.Task?[0]!;

        Assert.Equal(hideUntil, taskModel.HideUntil);

        Assert.Equal(hideInMilliseconds, taskFromModel.HideUntil);
        Assert.Equal(hide, taskFromModel.Hide);
    }

    private (TaskAppDataModel? model, GTDDataModel? fromModel) GetMappedInfo(GTDDataModel gtdDataModel)
    {
        if (gtdDataModel == null)
            return (null, null);
        var taskAppDataModel = TestConverter.MapToModel(gtdDataModel);
        var gtdDataMappedRemappedModel = TestConverter.MapFromModel(taskAppDataModel);

        return (taskAppDataModel, gtdDataMappedRemappedModel);
    }
}