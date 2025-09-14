using Ical.Net;
using TaskConverter.Commons;

namespace TaskConverter.Plugin.GTD.Conversion;

public interface IConversionService<T>
    where T : class
{
    ISettingsProvider SettingsProvider { get; }
    Calendar MapToIntermediateFormat(T taskInfo);
    T MapFromIntermediateFormat(Calendar model);
}
