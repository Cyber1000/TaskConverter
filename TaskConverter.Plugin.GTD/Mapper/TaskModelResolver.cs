using AutoMapper;
using TaskConverter.Model.Model;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Mapper;

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
