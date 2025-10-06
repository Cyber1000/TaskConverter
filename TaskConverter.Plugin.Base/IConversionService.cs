using Ical.Net;

namespace TaskConverter.Plugin.Base;

public interface IConversionService<T>
    where T : class
{
    ISettingsProvider SettingsProvider { get; }
    Calendar MapToIntermediateFormat(T taskInfo);
    T MapFromIntermediateFormat(Calendar model);
}
