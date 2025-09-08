using Ical.Net.CalendarComponents;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.TodoModel;

namespace TaskConverter.Plugin.GTD.Conversion;

public class MapKeyWordsOfTodoToIntermediateFormat : MapKeyWordsToIntermediateFormatBase<GTDTaskModel, Todo>
{
    protected override IEnumerable<(int Id, KeyWordType keyWordType)> GetKeyWords(GTDTaskModel source)
    {
        return new[] { (Id: source.Context, KeyWordType: KeyWordType.Context), (Id: source.Folder, KeyWordType: KeyWordType.Folder) }
            .Where(keyWord => keyWord.Id != 0)
            .Concat(source.Tag?.Select(t => (Id: t, KeyWordType: KeyWordType.Tag)) ?? Enumerable.Empty<(int Id, KeyWordType KeyWordType)>());
    }
}
