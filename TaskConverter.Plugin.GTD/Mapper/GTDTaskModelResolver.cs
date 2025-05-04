using AutoMapper;
using Ical.Net;
using Ical.Net.CalendarComponents;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.TodoModel;
using TaskConverter.Plugin.GTD.Utils;

namespace TaskConverter.Plugin.GTD.Mapper;

public class GTDTaskModelResolver : IValueResolver<Calendar, GTDDataModel, List<GTDTaskModel>?>
{
    public List<GTDTaskModel>? Resolve(Calendar source, GTDDataModel destination, List<GTDTaskModel>? destMember, ResolutionContext resolutionContext)
    {
        var timeZone = resolutionContext.GetTimeZone();
        var todos = GetTodos(source);
        return todos.Select(todo =>
                {
                    var propertiesOfTodo = todo.Properties.ToDictionary(p => p.Name);
                    var keyWordMetaData = propertiesOfTodo.GetKeyWordMetaData(todo.Categories, timeZone);

                    var tags = keyWordMetaData.Where(k => k.KeyWordType == KeyWordType.Tag)?.Select(k => k.Id).ToList() ?? [];
                    var folder = keyWordMetaData.Where(k => k.KeyWordType == KeyWordType.Folder)?.SingleOrDefault().Id ?? 0;
                    var context = keyWordMetaData.Where(k => k.KeyWordType == KeyWordType.Context)?.SingleOrDefault().Id ?? 0;

                    var foreignId = (todo.Parent as Todo)?.Uid;
                    //TODO HH: use from StringToIntConverter
                    int parentId = int.TryParse(foreignId, out var parentParsedId) ? parentParsedId : Math.Abs(foreignId?.GetHashCode() ?? 0);

                    var model = new GTDTaskModel
                    {
                        Parent = parentId,
                        Tag = tags,
                        Folder = folder,
                        Context = context,
                    };
                    return resolutionContext.Mapper.Map(todo, model);
                })
                .ToList() ?? [];
    }

    private static List<Todo> GetTodos(Calendar source)
    {
        var todos = new List<Todo>();
        AddTodos(source, todos);
        return todos;

        static void AddTodos(ICalendarObject calendarObject, List<Todo> todos)
        {
            foreach (var calendarItem in calendarObject.Children)
            {
                if (calendarItem is Todo todo)
                    todos.Add(todo);

                AddTodos(calendarItem, todos);
            }
        }
    }
}
