using AutoMapper;
using TaskConverter.Model.Model;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Mapper;

public class NotebookModelResolver : IValueResolver<GTDDataModel, TaskAppDataModel, List<NotebookModel>?>
{
    public List<NotebookModel>? Resolve(
        GTDDataModel source,
        TaskAppDataModel destination,
        List<NotebookModel>? destMember,
        ResolutionContext context
    ) =>
        source.Notebook
            ?.Select(s =>
            {
                var model = new NotebookModel(destination, s.FolderId == 0 ? null : s.FolderId.ToString());
                return context.Mapper.Map(s, model);
            })
            .ToList() ?? [];
}
