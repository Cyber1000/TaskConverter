using AutoMapper;
using Ical.Net;
using Ical.Net.CalendarComponents;
using TaskConverter.Plugin.Base.ConversionHelper;
using TaskConverter.Plugin.Base.Utils;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.TodoModel;
using TaskConverter.Plugin.GTD.Utils;

namespace TaskConverter.Plugin.GTD.Conversion;

public class MapTodoFromIntermediateFormat : IMappingAction<Calendar, GTDDataModel>
{
    public void Process(Calendar source, GTDDataModel destination, ResolutionContext resolutionContext)
    {
        var settingsProvider = resolutionContext.GetSettingsProvider();
        var statusSymbol = settingsProvider.GetIntermediateFormatSymbol(KeyWordType.Status);

        var keyWordMetaDataList = resolutionContext.GetKeyWordMetaDataIntermediateFormatDictionary();
        var todos = GetTodos(source);
        destination.Task =
            todos
                .Select(todo =>
                {
                    var keyWordMetaDataForCurrentTodo = todo.Categories.GetExistingValues(keyWordMetaDataList);

                    var tags = keyWordMetaDataForCurrentTodo.Where(k => k.KeyWordType == KeyWordType.Tag)?.Select(k => k.Id).ToList() ?? [];
                    var folder = keyWordMetaDataForCurrentTodo.Where(k => k.KeyWordType == KeyWordType.Folder)?.SingleOrDefault().Id ?? 0;
                    var context = keyWordMetaDataForCurrentTodo.Where(k => k.KeyWordType == KeyWordType.Context)?.SingleOrDefault().Id ?? 0;
                    var status = keyWordMetaDataForCurrentTodo.Where(k => k.KeyWordType == KeyWordType.Status)?.SingleOrDefault();

                    var statusName = status?.Name ?? string.Empty;
                    var statusId = status?.Id ?? 0;
                    statusName = statusName.RemovePrefix(statusSymbol);

                    var statusEnum = Status.None;
                    if (statusId > 0 && !Enum.TryParse(statusName, ignoreCase: true, out statusEnum))
                        tags.Add(statusId);

                    if (statusEnum == Status.None)
                        statusEnum = todo.Status.MapStatus();

                    var parentId = GetParentId(todo);

                    var model = new GTDTaskModel
                    {
                        Parent = parentId,
                        Tag = tags,
                        Folder = folder,
                        Context = context,
                        Status = statusEnum,
                    };
                    return resolutionContext.Mapper.Map(todo, model);
                })
                .ToList() ?? [];

        static int GetParentId(Todo todo)
        {
            var parentProp = todo.Properties.FirstOrDefault(p =>
                string.Equals(p.Name, "RELATED-TO", StringComparison.OrdinalIgnoreCase)
                && (
                    p.Parameters.Any(param => string.Equals(param.Name, "RELTYPE", StringComparison.OrdinalIgnoreCase) && string.Equals(param.Value, "PARENT", StringComparison.OrdinalIgnoreCase))
                    || !p.Parameters.Any(param => string.Equals(param.Name, "RELTYPE", StringComparison.OrdinalIgnoreCase))
                )
            );

            return parentProp?.Value?.ToString().ToIntWithHashFallback() ?? 0;
        }
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
