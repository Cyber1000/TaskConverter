using System.Xml;

namespace TaskConverter.Model.Model;

public class TaskAppDataModel
{
    public int Version { get; set; }

    public List<KeyWordModel>? KeyWords { get; set; }

    public List<TaskModel>? Tasks { get; set; }

    public List<NotebookModel>? Notebooks { get; set; }
}
