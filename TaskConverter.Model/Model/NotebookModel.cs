using System.Diagnostics.CodeAnalysis;

namespace TaskConverter.Model.Model;

public class NotebookModel(TaskAppDataModel taskAppDataModel, [AllowNull] string keyWordId) : ExtendedModel
{
    private readonly Func<Dictionary<string, KeyWordModel>> KeyWordsFunc = () => taskAppDataModel.KeyWords?.ToDictionary(f => f.Id) ?? [];

    //TODO HH: string instead?
    public string[]? Note { get; set; }

    public KeyWordModel? Keyword => string.IsNullOrEmpty(keyWordId) ? null : KeyWordsFunc.Invoke()[keyWordId];
}
