using AutoMapper;
using Ical.Net;
using Ical.Net.CalendarComponents;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Conversion;

public class MapTodosToIntermediateFormat : IMappingAction<GTDDataModel, Calendar>
{
    public void Process(GTDDataModel source, Calendar destination, ResolutionContext context)
    {
        var sourceTasks = source.Task ?? Enumerable.Empty<GTDTaskModel>();

        foreach (var sourceTask in sourceTasks)
        {
            var todo = context.Mapper.Map<Todo>(sourceTask);

            if (sourceTask.Parent != 0)
            {
                var rel = new CalendarProperty("RELATED-TO", sourceTask.Parent.ToString())
                {
                    Parameters = { new CalendarParameter("RELTYPE", "PARENT") }
                };
                todo.Properties.Add(rel);
            }

            destination.Todos.Add(todo);
        }
    }
}
