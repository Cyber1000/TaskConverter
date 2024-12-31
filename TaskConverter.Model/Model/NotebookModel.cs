using System.Diagnostics.CodeAnalysis;

namespace TaskConverter.Model.Model;

public class NotebookModel(IModelWithKeyWordsFactory factory, [AllowNull] string keyWordId) : ExtendedModel
{
    public string? Note { get; set; }

    public KeyWordModel? KeyWord => string.IsNullOrEmpty(keyWordId) ? null : factory.GetKeyWords()[keyWordId];
}
