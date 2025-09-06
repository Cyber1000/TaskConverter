using AutoMapper;
using Ical.Net;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.TodoModel;
using TaskConverter.Plugin.GTD.Utils;

namespace TaskConverter.Plugin.GTD.Conversion;

public class GTDKeyWordMappingAction : IMappingAction<Calendar, GTDDataModel>
{
    public void Process(Calendar source, GTDDataModel destination, ResolutionContext resolutionContext)
    {
        var timeZone = resolutionContext.GetTimeZone();
        var keyWordMetaData = source
            .Todos.Select(t => t.GetKeyWordMetaDataList(timeZone))
            .Union(source.Journals.Select(j => j.GetKeyWordMetaDataList(timeZone)))
            .SelectMany(t => t)
            .Distinct()
            .ToList();

        var tags = keyWordMetaData.Where(k => k.KeyWordType == KeyWordType.Tag)?.Select(k => resolutionContext.Mapper.Map<GTDTagModel>(k)).ToList() ?? [];
        var folder = keyWordMetaData.Where(k => k.KeyWordType == KeyWordType.Folder)?.Select(k => resolutionContext.Mapper.Map<GTDFolderModel>(k)).ToList() ?? [];
        var contexts = keyWordMetaData.Where(k => k.KeyWordType == KeyWordType.Context)?.Select(k => resolutionContext.Mapper.Map<GTDContextModel>(k)).ToList() ?? [];

        destination.Tag = tags;
        destination.Folder = folder;
        destination.Context = contexts;
    }
}
