using Ical.Net;
using NodaTime;
using TaskConverter.Commons;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.TodoModel;

namespace TaskConverter.Plugin.GTD.Conversion
{
    public interface IKeyWordMapperService
    {
        Dictionary<(KeyWordType keyWordType, int Id), KeyWordMetaData> GetKeyWordMetaDataGTDFormatDictionary(GTDDataModel gtdDataModel, ISettingsProvider settingsProvider);

        Dictionary<string, KeyWordMetaData> GetKeyWordMetaDataIntermediateFormatDictionary(Calendar calender, ISettingsProvider settingsProvider);
    }
}
