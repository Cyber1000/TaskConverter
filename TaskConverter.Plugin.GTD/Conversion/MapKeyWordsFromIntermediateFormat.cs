using AutoMapper;
using Ical.Net;
using TaskConverter.Plugin.Base.Utils;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.TodoModel;
using TaskConverter.Plugin.GTD.Utils;

namespace TaskConverter.Plugin.GTD.Conversion;

public class MapKeyWordsFromIntermediateFormat : IMappingAction<Calendar, GTDDataModel>
{
    public void Process(Calendar source, GTDDataModel destination, ResolutionContext resolutionContext)
    {
        var settingsProvider = resolutionContext.GetSettingsProvider();
        var statusSymbol = settingsProvider.GetIntermediateFormatSymbol(KeyWordType.Status);
        var keyWordMetaDataList = resolutionContext.GetKeyWordMetaDataIntermediateFormatDictionary();

        var tags = keyWordMetaDataList?.Values.Where(k => k.KeyWordType == KeyWordType.Tag)?.Select(k => Map<GTDTagModel>(k)).ToList() ?? [];
        var folder = keyWordMetaDataList?.Values.Where(k => k.KeyWordType == KeyWordType.Folder)?.Select(k => Map<GTDFolderModel>(k)).ToList() ?? [];
        var contexts = keyWordMetaDataList?.Values.Where(k => k.KeyWordType == KeyWordType.Context)?.Select(k => Map<GTDContextModel>(k)).ToList() ?? [];

        var statusEnumNames = new HashSet<string>(Enum.GetNames<Status>(), StringComparer.OrdinalIgnoreCase);
        var filteredStatus =
            keyWordMetaDataList
                ?.Values.Where(k => k.KeyWordType == KeyWordType.Status && !statusEnumNames.Contains(k.Name.RemovePrefix(statusSymbol)))
                .Select(Map<GTDTagModel>)
                .ToList() ?? [];

        destination.Tag = tags.Union(filteredStatus).ToList();
        destination.Folder = folder;
        destination.Context = contexts;

        T Map<T>(KeyWordMetaData keyWordMetaData)
            where T : GTDBaseModel
        {
            var model = resolutionContext.Mapper.Map<T>(keyWordMetaData);
            model.Title = KeyWordMapperService.MapKeyWordNameToGTDFormat(keyWordMetaData.Name, keyWordMetaData.KeyWordType, settingsProvider);
            return model;
        }
    }
}
