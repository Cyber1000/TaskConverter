using AutoMapper;
using TaskConverter.Model.Model;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Mapper;

//TODO HH: add base
public class KeyWordModelResolver: IValueResolver<GTDDataModel, TaskAppDataModel, List<KeyWordModel>?>
{
    public List<KeyWordModel>? Resolve(GTDDataModel source, TaskAppDataModel destination, List<KeyWordModel>? destMember, ResolutionContext resolutionContext)
    {
        var mapper = resolutionContext.Mapper;
        return [.. MapToKeyWordModels(source.Tag, mapper), .. MapToKeyWordModels(source.Folder, mapper), .. MapToKeyWordModels(source.Context, mapper)];
    }

    private static IEnumerable<KeyWordModel> MapToKeyWordModels<T>(IEnumerable<T>? source, IRuntimeMapper mapper)
    {
        return source?.Select(item => mapper.Map<KeyWordModel>(item)) ?? [];
    }
}