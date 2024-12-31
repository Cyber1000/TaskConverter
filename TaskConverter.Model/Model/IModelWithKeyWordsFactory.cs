namespace TaskConverter.Model.Model;

public interface IModelWithKeyWordsFactory
{
    Dictionary<string, KeyWordModel> GetKeyWords();
}
