using TaskConverter.Model.Mapper;
using TaskConverter.Model.Model;
using TaskConverter.Plugin.GTD.Model;
using NodaTime;

namespace TaskConverter.Plugin.GTD.Mapper;

//TODO HH: move to base?
public interface IConverter
{
    IClock Clock { get; }
    IConverterDateTimeZoneProvider DateTimeZoneProvider { get; }
    TaskAppDataModel MapToModel(GTDDataModel taskInfo);
    GTDDataModel MapFromModel(TaskAppDataModel model);
}

public class Converter(IClock clock, IConverterDateTimeZoneProvider dateTimeZoneProvider) : IConverter
{
    private readonly GTDMapper GTDMapper = new(clock, dateTimeZoneProvider);
    public IClock Clock { get; } = clock;
    public IConverterDateTimeZoneProvider DateTimeZoneProvider { get; set; } = dateTimeZoneProvider;

    public TaskAppDataModel MapToModel(GTDDataModel taskInfo) => GTDMapper.Mapper.Map<TaskAppDataModel>(taskInfo);

    public GTDDataModel MapFromModel(TaskAppDataModel model) => GTDMapper.Mapper.Map<GTDDataModel>(model);
}
