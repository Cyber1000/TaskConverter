using AutoMapper;
using TaskConverter.Model.Model;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Mapper;

public class GTDTaskModelResolver : IValueResolver<TaskAppDataModel, GTDDataModel, List<GTDTaskModel>?>
{
    public List<GTDTaskModel>? Resolve(TaskAppDataModel source, GTDDataModel destination, List<GTDTaskModel>? destMember, ResolutionContext resolutionContext) =>
        source.Tasks
            ?.Select(s =>
            {
                var tags = s.KeyWords?.Where(k => k.KeyWordType == KeyWordEnum.Tag).Select(t => int.Parse(t.Id)).ToList() ?? [];
                var folder = int.TryParse(s.KeyWords?.FirstOrDefault(k => k.KeyWordType == GTDKeyWordEnum.Folder)?.Id, out var folderParsedId) ? folderParsedId : 0;
                var context = int.TryParse(s.KeyWords?.FirstOrDefault(k => k.KeyWordType == GTDKeyWordEnum.Context)?.Id, out var contextParsedId) ? contextParsedId : 0;

                int parentId = int.TryParse(s.Parent?.Id, out var parentParsedId) ? parentParsedId : 0;

                //TODO HH: fix
                var model = new GTDTaskModel(destination, parentId, tags, folder, context);
                return resolutionContext.Mapper.Map(s, model);
            }).ToList() ?? [];
}
