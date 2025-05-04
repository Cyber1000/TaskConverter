using NodaTime;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Tests.Utils;

namespace TaskConverter.Plugin.GTD.Tests.MappingTests;

public static class GTDTaskModelBuilderExtensions
{
    public static GTDTaskModelBuilder GTDTaskModel(this IObjectBuilder _, int id) => new(id);
}

public class GTDTaskModelBuilder
{
    private readonly int _id;
    private int parentId;
    private int folderId;
    private int contextId;
    private List<int> tagIds = [];
    private LocalDateTime? dueDate;
    private GTDRepeatInfoModel? repeatNew;
    private bool? floating;
    private long? reminder;
    private GTDRepeatFrom repeatFrom;
    private Hide? hide;
    private long? hideUntil;

    public GTDTaskModelBuilder(int id)
    {
        _id = id;
        WithParent(0);
        WithFolder(0);
        WithContext(0);
        WithTags([]);
        WithDueDate(new LocalDateTime(2023, 02, 23, 0, 0, 0));
        WithRepeat(new GTDRepeatInfoModel("Every 1 week"), GTDRepeatFrom.FromDueDate);
        WithFloating(false);
        WithReminder(-1);
        WithHide(Hide.GivenDate);
        WithHideUntil(1677402000000);
    }

    public GTDTaskModelBuilder WithParent(int parent)
    {
        parentId = parent;
        return this;
    }

    public GTDTaskModelBuilder WithFolder(int folder)
    {
        folderId = folder;
        return this;
    }

    public GTDTaskModelBuilder WithContext(int context)
    {
        contextId = context;
        return this;
    }

    public GTDTaskModelBuilder WithTags(List<int> tags)
    {
        tagIds = tags;
        return this;
    }

    public GTDTaskModelBuilder WithDueDate(LocalDateTime? date)
    {
        dueDate = date;
        return this;
    }

    public GTDTaskModelBuilder WithRepeat(GTDRepeatInfoModel? repeatNew, GTDRepeatFrom repeatFrom)
    {
        this.repeatNew = repeatNew;
        this.repeatFrom = repeatFrom;
        return this;
    }

    public GTDTaskModelBuilder WithFloating(bool isFloating)
    {
        floating = isFloating;
        return this;
    }

    public GTDTaskModelBuilder WithReminder(long reminderValue)
    {
        reminder = reminderValue;
        return this;
    }

    public GTDTaskModelBuilder WithHide(Hide hideValue)
    {
        hide = hideValue;
        return this;
    }

    public GTDTaskModelBuilder WithHideUntil(long hideUntilValue)
    {
        hideUntil = hideUntilValue;
        return this;
    }

    public GTDTaskModel Build()
    {
        return new()
        {
            Id = _id,
            Uuid = "",
            Parent = parentId,
            Created = new LocalDateTime(2023, 02, 20, 10, 0, 0),
            Modified = new LocalDateTime(2023, 02, 21, 10, 0, 0),
            Title = $"Task {_id}",
            StartDate = null,
            StartTimeSet = false,
            DueDate = dueDate,
            DueDateProject = null,
            DueTimeSet = false,
            DueDateModifier = DueDateModifier.DueBy,
            Reminder = reminder ?? -1,
            Alarm = null,
            RepeatNew = repeatNew,
            RepeatFrom = repeatFrom,
            Duration = 0,
            Status = Plugin.GTD.Model.Status.NextAction,
            Context = contextId,
            Goal = 0,
            Folder = folderId,
            Tag = tagIds,
            Starred = true,
            Priority = Plugin.GTD.Model.Priority.Low,
            Note = ["Note"],
            Completed = new LocalDateTime(2023, 02, 25, 10, 0, 0),
            Type = Plugin.GTD.Model.TaskType.Task,
            TrashBin = "",
            Importance = 0,
            MetaInformation = "",
            Floating = floating ?? false,
            Hide = hide ?? Hide.DontHide,
            HideUntil = hideUntil ?? 0,
        };
    }
}
