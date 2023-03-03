using Converter.Core.GTD.Model;

namespace Converter.Core.GTD.InternalModel
{
    public class TaskModel : BaseModel
    {
        private readonly Func<Dictionary<int, TaskModel>> TasksFunc;
        private readonly Func<Dictionary<int, ContextModel>> ContextsFunc;
        private readonly Func<Dictionary<int, FolderModel>> FoldersFunc;
        private readonly Func<Dictionary<int, TagModel>> TagsFunc;

        private readonly int ParentId;
        private readonly int ContextId;
        private readonly int FolderId;
        private readonly List<int> TagIds;

        public TaskModel(TaskInfoModel taskInfoModel, int parentId, int contextId, int folderId, List<int> tagIds)
        {
            TasksFunc = () => taskInfoModel.Tasks?.ToDictionary(p => p.Id) ?? new Dictionary<int, TaskModel>();
            ContextsFunc = () => taskInfoModel.Contexts?.ToDictionary(p => p.Id) ?? new Dictionary<int, ContextModel>();
            FoldersFunc = () => taskInfoModel.Folders?.ToDictionary(p => p.Id) ?? new Dictionary<int, FolderModel>();
            TagsFunc = () => taskInfoModel.Tags?.ToDictionary(p => p.Id) ?? new Dictionary<int, TagModel>();
            ParentId = parentId;
            ContextId = contextId;
            FolderId = folderId;
            TagIds = tagIds;
        }

        public TaskModel? Parent => ParentId == 0 ? null : TasksFunc.Invoke()[ParentId];
        public DateTime? DueDate { get; set; }
        public DateTime? DueDateProject { get; set; }

        //TODO HH: extra testen (und entfernen) -> nur gesetzt wenn Zeit DueDate ein Datum mit Zeit
        public bool DueTimeSet { get; set; }

        //TODO HH: generieren -> 3 wenn floating, 0 sonst
        public DueDateModifier DueDateModifier { get; set; }
        public long Reminder { get; set; }

        //TODO HH: generieren -> wenn Reminder vor aktueller Uhrzeit
        public DateTime? Alarm { get; set; }

        //TODO HH: anpassen
        //TODO HH: testen, wenn null (RepeatFrom mappen)
        public RepeatInfoModel? RepeatInfo { get; set; }
        public Status Status { get; set; }
        public ContextModel? Context => ContextId == 0 ? null : ContextsFunc.Invoke()[ContextId];

        public FolderModel? Folder => FolderId == 0 ? null : FoldersFunc.Invoke()[FolderId];
        public List<TagModel> Tags => TagIds == null ? new List<TagModel>() : TagIds.Select(t => TagsFunc.Invoke()[t]).ToList();
        public bool Starred { get; set; }
        public Priority Priority { get; set; }
        public string[]? Note { get; set; }
        public DateTime? Completed { get; set; }
        public TaskType Type { get; set; }
        public bool Floating { get; set; }

        //TODO HH: eventuell anpassen
        public Hide Hide { get; set; }
        public DateTime? HideUntil { get; set; }
    }
}
