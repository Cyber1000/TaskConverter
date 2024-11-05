using NodaTime;

namespace TaskConverter.Model.Model
{
    public class TaskModel(TaskInfoModel taskInfoModel, int parentId, int contextId, int folderId, List<int> tagIds) : BaseModel
    {
        private readonly Func<Dictionary<int, TaskModel>> TasksFunc = () => taskInfoModel.Tasks?.ToDictionary(p => p.Id) ?? [];
        private readonly Func<Dictionary<int, ContextModel>> ContextsFunc = () => taskInfoModel.Contexts?.ToDictionary(p => p.Id) ?? [];
        private readonly Func<Dictionary<int, FolderModel>> FoldersFunc = () => taskInfoModel.Folders?.ToDictionary(p => p.Id) ?? [];
        private readonly Func<Dictionary<int, TagModel>> TagsFunc = () => taskInfoModel.Tags?.ToDictionary(p => p.Id) ?? [];

        private readonly int ParentId = parentId;
        private readonly int ContextId = contextId;
        private readonly int FolderId = folderId;
        private readonly List<int> TagIds = tagIds;

        public TaskModel? Parent => ParentId == 0 ? null : TasksFunc.Invoke()[ParentId];
        public LocalDateTime? DueDate { get; set; }
        public LocalDateTime? DueDateProject { get; set; }

        public ReminderInstant? Reminder { get; set; }

        public RepeatInfoModel? RepeatInfo { get; set; }
        public Status Status { get; set; }
        public ContextModel? Context => ContextId == 0 ? null : ContextsFunc.Invoke()[ContextId];

        public FolderModel? Folder => FolderId == 0 ? null : FoldersFunc.Invoke()[FolderId];
        public List<TagModel> Tags => TagIds == null ? [] : TagIds.Select(t => TagsFunc.Invoke()[t]).ToList();
        public bool Starred { get; set; }
        public Priority Priority { get; set; }
        public string[]? Note { get; set; }
        public LocalDateTime? Completed { get; set; }
        public TaskType Type { get; set; }
        public bool Floating { get; set; }
        public Instant? HideUntil { get; set; }
    }
}
