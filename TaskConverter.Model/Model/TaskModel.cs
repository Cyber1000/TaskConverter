using System.Diagnostics.CodeAnalysis;
using NodaTime;

namespace TaskConverter.Model.Model;

public class TaskModel(TaskAppDataModel taskAppDataModel, [AllowNull] string parentId, List<string> keyWordIds) : BaseModel
{
    private readonly Func<Dictionary<string, TaskModel>> TasksFunc = () => taskAppDataModel.Tasks?.ToDictionary(p => p.Id) ?? [];
    private readonly Func<Dictionary<string, KeyWordModel>> KeyWordsFunc = () => taskAppDataModel.KeyWords?.ToDictionary(p => p.Id) ?? [];

    public TaskModel? Parent => string.IsNullOrEmpty(parentId) ? null : TasksFunc.Invoke()[parentId];
    public LocalDateTime? DueDate { get; set; }
    
    //TODO HH: DueDateProject needed?
    public LocalDateTime? DueDateProject { get; set; }
    public bool HasFloatingDueDate { get; set; }
    
    public ReminderInstant? Reminder { get; set; }
    public RepeatInfoModel? RepeatInfo { get; set; }

    public Status Status { get; set; }
    public List<KeyWordModel> KeyWords => keyWordIds.Count == 0 ? [] : keyWordIds.Select(t => KeyWordsFunc.Invoke()[t]).ToList();
    public bool Starred { get; set; }
    public Priority Priority { get; set; }
    //TODO HH: string instead?
    public string[]? Note { get; set; }
    public LocalDateTime? Completed { get; set; }
    public TaskType Type { get; set; }
    public Instant? HideUntil { get; set; }
}
