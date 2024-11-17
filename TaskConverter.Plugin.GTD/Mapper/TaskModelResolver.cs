using AutoMapper;
using TaskConverter.Model.Model;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Mapper;

//TODO HH: add base / use mapper internally
public class TaskModelResolver : IValueResolver<GTDDataModel, TaskAppDataModel, List<TaskModel>?>
{
    public List<TaskModel>? Resolve(GTDDataModel source, TaskAppDataModel destination, List<TaskModel>? destMember, ResolutionContext resolutionContext) =>
        source.Task
            ?.Select(s =>
            {
                var keyWordList = new[] { s.Context, s.Folder }
                    .Where(keyWord => keyWord != 0)
                    .Select(keyWord => keyWord.ToString())
                    .Concat(s.Tag?.Select(t => t.ToString()) ?? [])
                    .ToList();

                var parentId = s.Parent == 0 ? null : s.Parent.ToString();

                var model = new TaskModel(destination, parentId, keyWordList);
                return resolutionContext.Mapper.Map(s, model);
            }).ToList() ?? [];
}
