using AutoMapper;
using Ical.Net;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.TodoModel;

namespace TaskConverter.Plugin.GTD.Conversion;

public class MapKeyWordsFromIntermediateFormat : IMappingAction<Calendar, GTDDataModel>
{
    public void Process(Calendar source, GTDDataModel destination, ResolutionContext resolutionContext)
    {
        var keyWordMetaDataList = resolutionContext.GetKeyWordMetaDataIntermediateFormatDictionary();

        var tags = keyWordMetaDataList?.Values.Where(k => k.KeyWordType == KeyWordType.Tag)?.Select(k => resolutionContext.Mapper.Map<GTDTagModel>(k)).ToList() ?? [];
        var folder = keyWordMetaDataList?.Values.Where(k => k.KeyWordType == KeyWordType.Folder)?.Select(k => resolutionContext.Mapper.Map<GTDFolderModel>(k)).ToList() ?? [];
        var contexts = keyWordMetaDataList?.Values.Where(k => k.KeyWordType == KeyWordType.Context)?.Select(k => resolutionContext.Mapper.Map<GTDContextModel>(k)).ToList() ?? [];

        destination.Tag = tags;
        destination.Folder = folder;
        destination.Context = contexts;
    }
}
