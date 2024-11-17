using AutoMapper;
using TaskConverter.Model.Model;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Mapper;

public class GTDNotebookModelResolver : IValueResolver<TaskAppDataModel, GTDDataModel, List<GTDNotebookModel>?>
{
    public List<GTDNotebookModel>? Resolve(TaskAppDataModel source, GTDDataModel destination, List<GTDNotebookModel>? destMember, ResolutionContext resolutionContext) =>
        source.Notebooks
            ?.Select(s =>
            {
                //TODO HH: fix
                var model = new GTDNotebookModel();
                return resolutionContext.Mapper.Map(s, model);
            }).ToList() ?? [];
}
