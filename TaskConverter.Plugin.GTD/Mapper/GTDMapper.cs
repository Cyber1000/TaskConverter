using System.Drawing;
using AutoMapper;
using NodaTime;
using TaskConverter.Model.Mapper;
using TaskConverter.Model.Model;
using TaskConverter.Commons.Utils;
using TaskConverter.Plugin.GTD.ConversionHelper;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Mapper;

public class GTDMapper
{
    public IMapper Mapper { get; }

    public GTDMapper(IClock clock, IConverterDateTimeZoneProvider dateTimeZoneProvider)
    {
        var timeZone = dateTimeZoneProvider.CurrentDateTimeZone;
        var config = new MapperConfiguration(cfg =>
        {
            //Map to Model
            cfg.CreateMap<GTDExtendedModel, ExtendedModel>().IncludeAllDerived().ForMember(dest => dest.Color, opt => opt.MapFrom(src => Color.FromArgb(src.Color)));

            cfg.CreateMap<GTDDataModel, TaskAppDataModel>()
                .ForMember(dest => dest.KeyWords, opt => opt.MapFrom<KeyWordModelResolver>())
                .ForMember(dest => dest.Tasks, opt => opt.MapFrom<TaskModelResolver>())
                .ForMember(dest => dest.Notebooks, opt => opt.MapFrom<NotebookModelResolver>())
                .ReverseMapWithValidation()
                .ForMember(dest => dest.Task, opt => opt.MapFrom<GTDTaskModelResolver>())
                .ForMember(dest => dest.Notebook, opt => opt.MapFrom<GTDNotebookModelResolver>())
                .ForMember(dest => dest.Preferences, opt => opt.Ignore())
                .ForMember(dest => dest.Tag, opt => opt.MapFrom(src => src.KeyWords == null ? new List<KeyWordModel>() : src.KeyWords.Where(k => k.KeyWordType == KeyWordEnum.Tag)))
                .ForMember(dest => dest.Folder, opt => opt.MapFrom(src => src.KeyWords == null ? new List<KeyWordModel>() : src.KeyWords.Where(k => k.KeyWordType == GTDKeyWordEnum.Folder)))
                .ForMember(dest => dest.Context, opt => opt.MapFrom(src => src.KeyWords == null ? new List<KeyWordModel>() : src.KeyWords.Where(k => k.KeyWordType == GTDKeyWordEnum.Context)))
                .ForMember(dest => dest.GetAllEntries, opt => opt.Ignore())
                .ForMember(dest => dest.TaskNote, opt => opt.Ignore());

            cfg.CreateMap<GTDTagModel, KeyWordModel>().ForMember(dest => dest.KeyWordType, opt => opt.MapFrom(src => KeyWordEnum.Tag));

            cfg.CreateMap<GTDFolderModel, KeyWordModel>().ForMember(dest => dest.KeyWordType, opt => opt.MapFrom(src => GTDKeyWordEnum.Folder));

            cfg.CreateMap<GTDContextModel, KeyWordModel>().ForMember(dest => dest.KeyWordType, opt => opt.MapFrom(src => GTDKeyWordEnum.Context));

            cfg.CreateMap<GTDTaskModel, TaskModel>()
                .DisableCtorValidation()
                .ForMember(dest => dest.RepeatInfo, opt => opt.MapFrom(src => CreateRepeatInfoModel(src.RepeatNew, src.RepeatFrom)))
                .ForMember(dest => dest.HideUntil, opt => opt.MapFrom(src => src.HideUntil == 0 ? (Instant?)null : Instant.FromUnixTimeMilliseconds(src.HideUntil)))
                .ForMember(dest => dest.Reminder, opt => opt.MapFrom(new ReminderInstantResolver(timeZone)))
                .ForMember(dest => dest.HasFloatingDueDate, opt => opt.MapFrom(src => src.Floating))
                .ForMember(dest => dest.KeyWords, opt => opt.Ignore())
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note != null ? src.Note.GetString() : null))
                .ReverseMapWithValidation()
                .ForMember(dest => dest.HideUntil, opt => opt.MapFrom(src => src.HideUntil.HasValue ? src.HideUntil.Value.ToUnixTimeMilliseconds() : 0))
                .ForMember(dest => dest.RepeatFrom, opt => opt.MapFrom(src => src.RepeatInfo.HasValue ? src.RepeatInfo.Value.RepeatFrom : RepeatFrom.FromDueDate))
                .ForMember(dest => dest.DueTimeSet, opt => opt.MapFrom(src => src.DueDate.HasValue && (src.DueDate.Value.TimeOfDay > new LocalTime())))
                .ForMember(dest => dest.DueDateModifier, opt => opt.MapFrom(src => src.HasFloatingDueDate ? DueDateModifier.OptionallyOn : DueDateModifier.DueBy))
                .ForMember(dest => dest.DueDateProject, opt => opt.Ignore())
                .ForMember(dest => dest.RepeatNew, opt => opt.MapFrom(src => CreateGTDRepeatInfoModel(src.RepeatInfo)))
                .ForMember(dest => dest.Reminder, opt => opt.MapFrom(new ReminderResolver(timeZone)))
                .ForMember(dest => dest.Alarm, opt => opt.MapFrom(new AlarmResolver(clock, timeZone)))
                .ForMember(dest => dest.Hide, opt => opt.MapFrom(new HideResolver(timeZone)))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note != null ? src.Note.GetStringArray() : null))
                .ForMember(dest => dest.StartDate, opt => opt.Ignore())
                .ForMember(dest => dest.StartTimeSet, opt => opt.Ignore())
                .ForMember(dest => dest.Duration, opt => opt.Ignore())
                .ForMember(dest => dest.Context, opt => opt.Ignore())
                .ForMember(dest => dest.Folder, opt => opt.Ignore())
                .ForMember(dest => dest.Tag, opt => opt.Ignore())
                .ForMember(dest => dest.Goal, opt => opt.Ignore())
                .ForMember(dest => dest.TrashBin, opt => opt.Ignore())
                .ForMember(dest => dest.Importance, opt => opt.Ignore())
                .ForMember(dest => dest.MetaInformation, opt => opt.Ignore())
                .ForMember(dest => dest.Parent, opt => opt.Ignore())
                .ForMember(dest => dest.Type, opt => opt.Ignore());

            cfg.CreateMap<GTDNotebookModel, NotebookModel>().DisableCtorValidation()
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note != null ? src.Note.GetString() : null));

            // Map from Model
            cfg.CreateMap<ExtendedModel, GTDExtendedModel>().IncludeAllDerived().ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color.ToArgb()));

            cfg.CreateMap<BaseModel, GTDBaseModel>().IncludeAllDerived().ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => string.Empty));

            cfg.CreateMap<KeyWordModel, GTDTagModel>().ForAllMembers(opts => opts.Condition((src, _, _) => src.KeyWordType == KeyWordEnum.Tag));

            cfg.CreateMap<KeyWordModel, GTDFolderModel>()
                .ForMember(dest => dest.Children, opt => opt.Ignore())
                .ForMember(dest => dest.Ordinal, opt => opt.Ignore())
                .ForMember(dest => dest.Parent, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, _, _) => src.KeyWordType == GTDKeyWordEnum.Folder));

            cfg.CreateMap<KeyWordModel, GTDContextModel>()
                .ForMember(dest => dest.Children, opt => opt.Ignore())
                .ForMember(dest => dest.Parent, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, _, _) => src.KeyWordType == GTDKeyWordEnum.Context));

            cfg.CreateMap<NotebookModel, GTDNotebookModel>()
                .ForMember(dest => dest.FolderId, opt => opt.MapFrom(src => src.KeyWord == null ? "0" : src.KeyWord.Id))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note != null ? src.Note.GetStringArray() : null))
                .ForMember(dest => dest.Private, opt => opt.Ignore());

            cfg.CreateMap<LocalDateTime, Instant>().ConvertUsing(s => s.InZoneLeniently(timeZone).ToInstant());
            cfg.CreateMap<Instant, LocalDateTime>().ConvertUsing(s => s.InZone(timeZone).LocalDateTime);
        });
        Mapper = config.CreateMapper();
    }

    private static RepeatInfoModel? CreateRepeatInfoModel(GTDRepeatInfoModel? repeatInfo, GTDRepeatFrom repeatFrom)
    {
        return repeatInfo.HasValue ? new RepeatInfoModel(repeatInfo.Value.Interval, repeatInfo.Value.Period, (RepeatFrom)repeatFrom) : null;
    }

    private static GTDRepeatInfoModel? CreateGTDRepeatInfoModel(RepeatInfoModel? repeatInfo)
    {
        return repeatInfo.HasValue ? new GTDRepeatInfoModel(repeatInfo.Value.Interval, repeatInfo.Value.Period) : null;
    }
}
