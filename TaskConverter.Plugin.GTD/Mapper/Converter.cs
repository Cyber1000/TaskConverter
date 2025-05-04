using Ical.Net;
using NodaTime;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Mapper;

//TODO HH: move to base?
public interface IConverter
{
    IClock Clock { get; }
    IConverterDateTimeZoneProvider DateTimeZoneProvider { get; }
    Calendar MapToModel(GTDDataModel taskInfo);
    GTDDataModel MapFromModel(Calendar model);
}

public class Converter(IClock clock, IConverterDateTimeZoneProvider dateTimeZoneProvider) : IConverter
{
    private readonly GTDMapper GTDMapper = new(clock, dateTimeZoneProvider);
    public IClock Clock { get; } = clock;
    public IConverterDateTimeZoneProvider DateTimeZoneProvider { get; set; } = dateTimeZoneProvider;

    public Calendar MapToModel(GTDDataModel taskInfo) => GTDMapper.MapToCalendar(taskInfo);

    public GTDDataModel MapFromModel(Calendar model) => GTDMapper.MapToGTDDataModel(model);
}
