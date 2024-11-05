namespace TaskConverter.Model.Model;

public class NotebookModel(TaskInfoModel taskInfoModel, int folderId) : ExtendedModel
{
    private readonly Func<Dictionary<int, FolderModel>> FoldersFunc = () => taskInfoModel.Folders?.ToDictionary(f => f.Id) ?? [];

    public string[]? Note { get; set; }

    public FolderModel? Folder => folderId == 0 ? null : FoldersFunc.Invoke()[folderId];
}
