using Ical.Net;

namespace TaskConverter.Plugin.GTD.Mapper;

public interface IConversionService<T>
    where T : class
{
    Calendar MapToIntermediateFormat(T taskInfo);
    T MapFromIntermediateFormat(Calendar model);
}
