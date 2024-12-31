namespace TaskConverter.Model.Model;

public class TaskModelContext(TaskAppDataModel model) : ModelWithKeyWordsContext(model), ITaskModelFactory
{
    public Dictionary<string, TaskModel> GetTasks() => Model.Tasks?.ToDictionary(p => p.Id) ?? [];
}
