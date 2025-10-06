using Ical.Net;
using NodaTime;
using TaskConverter.Plugin.Base;
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
