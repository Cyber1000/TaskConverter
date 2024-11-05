using AutoMapper;
using TaskConverter.Model.Model;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Mapper;

public class NotebookModelResolver : IValueResolver<TaskInfo, TaskInfoModel, List<NotebookModel>?>
{
    public List<NotebookModel>? Resolve(
        TaskInfo source,
        TaskInfoModel destination,
        List<NotebookModel>? destMember,
        ResolutionContext context
    ) =>
        source.Notebook
            ?.Select(s =>
            {
                var model = new NotebookModel(destination, s.FolderId);
                return context.Mapper.Map(s, model);
            })
            .ToList() ?? [];
}
