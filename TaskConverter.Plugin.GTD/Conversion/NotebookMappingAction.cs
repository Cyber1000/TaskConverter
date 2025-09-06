using AutoMapper;
using Ical.Net;
using Ical.Net.CalendarComponents;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Conversion;

public class NotebookMappingAction : IMappingAction<GTDDataModel, Calendar>
{
    public void Process(GTDDataModel source, Calendar destination, ResolutionContext context)
    {
        var journals = source.Notebook?.Select(context.Mapper.Map<Journal>).ToList() ?? [];

        journals.ForEach(destination.Journals.Add);
    }
}
