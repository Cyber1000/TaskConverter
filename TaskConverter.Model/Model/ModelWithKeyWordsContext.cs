namespace TaskConverter.Model.Model;

public class ModelWithKeyWordsContext(TaskAppDataModel model) : IModelWithKeyWordsFactory
{
    protected TaskAppDataModel Model => model;
    public Dictionary<string, KeyWordModel> GetKeyWords() => Model.KeyWords?.ToDictionary(p => p.Id) ?? [];
}
