using System.Xml;

namespace Converter.Core.GTD.InternalModel
{
    public class TaskInfoModel
    {
        public int Version { get; set; }

        public List<FolderModel>? Folders { get; set; }

        public List<ContextModel>? Contexts { get; set; }

        public List<TagModel>? Tags { get; set; }

        public List<TaskModel>? Tasks { get; set; }

        public List<NoteModel>? TaskNotes { get; set; }

        public List<NotebookModel>? Notebooks { get; set; }

        public XmlDocument? Config { get; set; }
    }
}
