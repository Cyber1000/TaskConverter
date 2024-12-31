using System.Diagnostics.CodeAnalysis;
using NodaTime;

namespace TaskConverter.Model.Model;


public class TaskModel(ITaskModelFactory factory, [AllowNull] string parentId, List<string> keyWordIds) : BaseModel
{
    public TaskModel? Parent => string.IsNullOrEmpty(parentId) ? null : factory.GetTasks()[parentId];
    public LocalDateTime? DueDate { get; set; }
    
    public bool HasFloatingDueDate { get; set; }
    
    public ReminderInstant? Reminder { get; set; }
    public RepeatInfoModel? RepeatInfo { get; set; }

    public Status Status { get; set; }
    public List<KeyWordModel> KeyWords => keyWordIds.Count == 0 ? [] : keyWordIds.Select(t => factory.GetKeyWords()[t]).ToList();
    public bool Starred { get; set; }
    public int Priority { get; set; }
    public string? Note { get; set; }
    public LocalDateTime? Completed { get; set; }
    public Instant? HideUntil { get; set; }
}
