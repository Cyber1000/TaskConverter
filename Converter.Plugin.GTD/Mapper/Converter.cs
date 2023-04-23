using Converter.Model.Mapper;
using Converter.Model.Model;
using Converter.Plugin.GTD.Model;
using NodaTime;

namespace Converter.Plugin.GTD.Mapper;

public interface IConverter
{
    IClock Clock { get; }
    IConverterDateTimeZoneProvider DateTimeZoneProvider { get; }
    TaskInfoModel MapToModel(TaskInfo taskInfo);
    TaskInfo MapFromModel(TaskInfoModel model);
}

public class Converter : IConverter
{
    private readonly GTDMapper GTDMapper;
    public IClock Clock { get; }
    public IConverterDateTimeZoneProvider DateTimeZoneProvider { get; set; }

    public Converter(IClock clock, IConverterDateTimeZoneProvider dateTimeZoneProvider)
    {
        DateTimeZoneProvider = dateTimeZoneProvider;
        Clock = clock;
        GTDMapper = new GTDMapper(clock, dateTimeZoneProvider);
    }

    public TaskInfoModel MapToModel(TaskInfo taskInfo)
    {
        return GTDMapper.Mapper.Map<TaskInfoModel>(taskInfo);
    }

    public TaskInfo MapFromModel(TaskInfoModel model)
    {
        return GTDMapper.Mapper.Map<TaskInfo>(model);
    }
}
