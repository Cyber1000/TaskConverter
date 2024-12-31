namespace TaskConverter.Model.Model;

public interface ITaskModelFactory : IModelWithKeyWordsFactory
{
    Dictionary<string, TaskModel> GetTasks();
}
