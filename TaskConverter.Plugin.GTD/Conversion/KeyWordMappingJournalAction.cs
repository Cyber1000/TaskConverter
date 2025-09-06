using Ical.Net.CalendarComponents;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.TodoModel;

namespace TaskConverter.Plugin.GTD.Conversion;

public class KeyWordMappingJournalAction : KeyWordMappingBaseAction<GTDNotebookModel, Journal>
{
    protected override IEnumerable<(int Id, KeyWordType keyWordType)> GetKeyWords(GTDNotebookModel source)
    {
        return source.FolderId > 0 ? [new(source.FolderId, KeyWordType.Folder)] : [];
    }
}
