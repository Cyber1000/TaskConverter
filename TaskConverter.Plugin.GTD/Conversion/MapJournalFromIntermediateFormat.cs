using AutoMapper;
using Ical.Net;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.TodoModel;
using TaskConverter.Plugin.GTD.Utils;

namespace TaskConverter.Plugin.GTD.Conversion;

public class MapJournalFromIntermediateFormat : IValueResolver<Calendar, GTDDataModel, List<GTDNotebookModel>?>
{
    public List<GTDNotebookModel>? Resolve(Calendar source, GTDDataModel destination, List<GTDNotebookModel>? destMember, ResolutionContext resolutionContext)
    {
        var timeZone = resolutionContext.GetTimeZone();
        return source
                .Journals?.Select(journal =>
                {
                    var propertiesOfTodo = journal.Properties.ToDictionary(p => p.Name);
                    var keyWordMetaData = propertiesOfTodo.GetKeyWordMetaData(journal.Categories, timeZone);
                    var folder = keyWordMetaData.Where(k => k.KeyWordType == KeyWordType.Folder)?.Single().Id ?? 0;

                    var model = new GTDNotebookModel { FolderId = folder };
                    return resolutionContext.Mapper.Map(journal, model);
                })
                .ToList() ?? [];
    }
}
