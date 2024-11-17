using System.Xml;

namespace TaskConverter.Model.Model;

public class TaskAppDataModel
{
    public int Version { get; set; }

    public List<KeyWordModel>? KeyWords { get; set; }

    public List<TaskModel>? Tasks { get; set; }

    //TODO HH: tasknotes is specific to GTD and should map within the task itself (for example additional notes or something like this)
    public List<NoteModel>? TaskNotes { get; set; }

    public List<NotebookModel>? Notebooks { get; set; }

    //TODO HH: needed?
    public XmlDocument? Config { get; set; }
}
