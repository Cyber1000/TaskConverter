using AutoMapper;
using Ical.Net;
using TaskConverter.Commons.Utils;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.TodoModel;

namespace TaskConverter.Plugin.GTD.Conversion;

public class MapJournalFromIntermediateFormat : IMappingAction<Calendar, GTDDataModel>
{
    public void Process(Calendar source, GTDDataModel destination, ResolutionContext resolutionContext)
    {
        var keyWordMetaDataList = resolutionContext.GetKeyWordMetaDataIntermediateFormatDictionary();
        destination.Notebook =
            source
                .Journals?.Select(journal =>
                {
                    var keyWordMetaDataForCurrentJournal = journal.Categories.GetExistingValues(keyWordMetaDataList);

                    var folder = keyWordMetaDataForCurrentJournal.Where(k => k.KeyWordType == KeyWordType.Folder)?.Single().Id ?? 0;

                    var model = new GTDNotebookModel { FolderId = folder };
                    return resolutionContext.Mapper.Map(journal, model);
                })
                .ToList() ?? [];
    }
}
