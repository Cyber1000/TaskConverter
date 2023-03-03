using AutoMapper;
using Converter.Core.GTD.InternalModel;
using Converter.Core.GTD.Model;

namespace Converter.Core.Mapper
{
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
                .ToList() ?? new List<NotebookModel>();
    }
}
