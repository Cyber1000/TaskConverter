using AutoMapper;
using Ical.Net;
using Ical.Net.CalendarComponents;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Conversion;

public class MapTodosToIntermediateFormat : IMappingAction<GTDDataModel, Calendar>
{
    public void Process(GTDDataModel source, Calendar destination, ResolutionContext context)
    {
        var todoDict =
            source.Task?.Select(sourceTask => (source: sourceTask, destination: context.Mapper.Map<Todo>(sourceTask)))
            .Where(taskInfo => taskInfo.destination.Uid is not null)
            .ToDictionary(taskInfo => taskInfo.destination.Uid!, taskInfo => taskInfo) ?? [];

        foreach (var todo in todoDict.Values)
        {
            var parentId = todo.source.Parent;
            if (parentId == 0)
            {
                destination.Todos.Add(todo.destination);
            }
            else if (todoDict.TryGetValue(parentId.ToString(), out var parentTodo))
            {
                parentTodo.destination.Children.Add(todo.destination);
            }
            else
            {
                throw new Exception($"Parent {parentId} not found");
            }
        }
    }
}
