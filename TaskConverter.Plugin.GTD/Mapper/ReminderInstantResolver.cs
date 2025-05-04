using AutoMapper;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using NodaTime;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Mapper;

public class ReminderInstantResolver() : IValueResolver<GTDTaskModel, Todo, Alarm?>
{
    public Alarm? Resolve(GTDTaskModel source, Todo destination, Alarm? destMember, ResolutionContext context)
    {
        if (source.Reminder > 43200)
            return new Alarm { Trigger = new Trigger { DateTime = new CalDateTime(DateTimeOffset.FromUnixTimeSeconds(source.Reminder).UtcDateTime) } };
        else if (source.Reminder >= 0)
            return new Alarm { Trigger = new Trigger { Duration = TimeSpan.FromMinutes(-source.Reminder) } };

        return null;
    }
}
