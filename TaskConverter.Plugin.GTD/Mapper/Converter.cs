using TaskConverter.Model.Mapper;
using TaskConverter.Model.Model;
using TaskConverter.Plugin.GTD.Model;
using NodaTime;

namespace TaskConverter.Plugin.GTD.Mapper;

public interface IConverter
{
    IClock Clock { get; }
    IConverterDateTimeZoneProvider DateTimeZoneProvider { get; }
    TaskInfoModel MapToModel(TaskInfo taskInfo);
    TaskInfo MapFromModel(TaskInfoModel model);
}

public class Converter(IClock clock, IConverterDateTimeZoneProvider dateTimeZoneProvider) : IConverter
{
    private readonly GTDMapper GTDMapper = new(clock, dateTimeZoneProvider);
    public IClock Clock { get; } = clock;
    public IConverterDateTimeZoneProvider DateTimeZoneProvider { get; set; } = dateTimeZoneProvider;

    public TaskInfoModel MapToModel(TaskInfo taskInfo) => GTDMapper.Mapper.Map<TaskInfoModel>(taskInfo);

    public TaskInfo MapFromModel(TaskInfoModel model) => GTDMapper.Mapper.Map<TaskInfo>(model);
}
