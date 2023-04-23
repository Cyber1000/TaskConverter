namespace Converter.Model.Model;

public class NotebookModel : ExtendedModel
{
    private readonly Func<Dictionary<int, FolderModel>> FoldersFunc;
    private readonly int FolderId;

    public NotebookModel(TaskInfoModel taskInfoModel, int folderId)
    {
        FoldersFunc = () => taskInfoModel.Folders?.ToDictionary(f => f.Id) ?? new Dictionary<int, FolderModel>();
        FolderId = folderId;
    }

    public string[]? Note { get; set; }

    public FolderModel? Folder => FolderId == 0 ? null : FoldersFunc.Invoke()[FolderId];
}
