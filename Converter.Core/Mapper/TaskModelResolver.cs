using AutoMapper;
using Converter.Core.GTD.InternalModel;
using Converter.Core.GTD.Model;

namespace Converter.Core.Mapper;

public class TaskModelResolver : IValueResolver<TaskInfo, TaskInfoModel, List<TaskModel>?>
{
    public List<TaskModel>? Resolve(TaskInfo source, TaskInfoModel destination, List<TaskModel>? destMember, ResolutionContext context) =>
        source.Task
            ?.Select(s =>
            {
                var model = new TaskModel(destination, s.Parent, s.Context, s.Folder, s.Tag);
                return context.Mapper.Map(s, model);
            })
            .ToList() ?? new List<TaskModel>();
}
