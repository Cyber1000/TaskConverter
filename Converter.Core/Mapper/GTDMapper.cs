using System.Drawing;
using AutoMapper;
using Converter.Core.GTD.InternalModel;
using Converter.Core.GTD.Model;
using Converter.Core.Utils;
using NodaTime;

namespace Converter.Core.Mapper
{
    public static class GTDMapper
    {
        public static IMapper Mapper { get; }

        static GTDMapper()
        {
            //TODO HH: improvement - Inject DateTimeZone
            var timeZone =
                DateTimeZoneProviders.Tzdb.GetZoneOrNull(SettingsHelper.GetAppSetting("TimeZoneId", ""))
                ?? DateTimeZoneProviders.Tzdb.GetSystemDefault();
            var config = new MapperConfiguration(cfg =>
            {
                //Map to Model
                cfg.CreateMap<TaskInfoExtendedEntry, ExtendedModel>()
                    .IncludeAllDerived()
                    .ForMember(dest => dest.Color, opt => opt.MapFrom(src => Color.FromArgb(src.Color)));

                cfg.CreateMap<TaskInfo, TaskInfoModel>()
                    .ForMember(dest => dest.Folders, opt => opt.MapFrom(src => src.Folder))
                    .ForMember(dest => dest.Contexts, opt => opt.MapFrom(src => src.Context))
                    .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tag))
                    .ForMember(dest => dest.Tasks, opt => opt.MapFrom<TaskModelResolver>())
                    .ForMember(dest => dest.TaskNotes, opt => opt.MapFrom(src => src.TaskNote))
                    .ForMember(dest => dest.Notebooks, opt => opt.MapFrom<NotebookModelResolver>())
                    .ForMember(
                        dest => dest.Config,
                        opt => opt.MapFrom(src => src.Preferences == null ? null : src.Preferences[0].XmlConfig)
                    )
                    .ReverseMap()
                    .ForMember(dest => dest.Notebook, opt => opt.MapFrom(src => src.Notebooks))
                    .ForMember(dest => dest.Task, opt => opt.MapFrom(src => src.Tasks))
                    .ForMember(
                        dest => dest.Preferences,
                        opt => opt.MapFrom(src => new List<Preferences>() { new Preferences { XmlConfig = src.Config } })
                    );

                cfg.CreateMap<TaskInfoFolderEntry, FolderModel>().ReverseMap();
                cfg.CreateMap<TaskInfoContextEntry, ContextModel>().ReverseMap();
                cfg.CreateMap<TaskInfoTagEntry, TagModel>().ReverseMap();
                cfg.CreateMap<TaskInfoTaskEntry, TaskModel>()
                    .ForMember(
                        dest => dest.RepeatInfo,
                        opt =>
                            opt.MapFrom(
                                src =>
                                    src.RepeatNew == null
                                        ? null
                                        : (RepeatInfoModel?)
                                            new RepeatInfoModel(src.RepeatNew!.Value.Interval, src.RepeatNew.Value.Period, src.RepeatFrom)
                            )
                    )
                    .ForMember(
                        dest => dest.HideUntil,
                        opt => opt.MapFrom(src => src.HideUntil == 0 ? (Instant?)null : Instant.FromUnixTimeMilliseconds(src.HideUntil))
                    )
                    .ForMember(dest => dest.Reminder, opt => opt.MapFrom(new ReminderInstantResolver(timeZone)))
                    .ReverseMap()
                    .ForMember(dest => dest.Parent, opt => opt.MapFrom(src => src.Parent == null ? 0 : src.Parent.Id))
                    .ForMember(dest => dest.Context, opt => opt.MapFrom(src => src.Context == null ? 0 : src.Context.Id))
                    .ForMember(dest => dest.Folder, opt => opt.MapFrom(src => src.Folder == null ? 0 : src.Folder.Id))
                    .ForMember(dest => dest.Tag, opt => opt.MapFrom(src => src.Tags == null ? new List<int>() : src.Tags.Select(t => t.Id)))
                    .ForMember(
                        dest => dest.HideUntil,
                        opt => opt.MapFrom(src => src.HideUntil.HasValue ? src.HideUntil.Value.ToUnixTimeMilliseconds() : 0)
                    )
                    .ForMember(
                        dest => dest.RepeatFrom,
                        opt => opt.MapFrom(src => src.RepeatInfo == null ? RepeatFrom.FromDueDate : src.RepeatInfo.Value.RepeatFrom)
                    )
                    .ForMember(
                        dest => dest.DueTimeSet,
                        opt => opt.MapFrom(src => src.DueDate.HasValue && (src.DueDate.Value.TimeOfDay > new LocalTime()))
                    )
                    .ForMember(
                        dest => dest.DueDateModifier,
                        opt => opt.MapFrom(src => src.Floating ? DueDateModifier.OptionallyOn : DueDateModifier.DueBy)
                    )
                    .ForMember(
                        dest => dest.RepeatNew,
                        opt =>
                            opt.MapFrom(
                                src =>
                                    src.RepeatInfo == null
                                        ? null
                                        : (RepeatInfo?)new RepeatInfo(src.RepeatInfo.Value.Interval, src.RepeatInfo.Value.Period)
                            )
                    )
                    .ForMember(dest => dest.Reminder, opt => opt.MapFrom(new ReminderResolver(timeZone)))
                    .ForMember(dest => dest.Alarm, opt => opt.MapFrom(new AlarmResolver(timeZone)))
                    .ForMember(dest => dest.Hide, opt => opt.MapFrom(new HideResolver(timeZone)));
                cfg.CreateMap<TaskInfoTaskNote, NoteModel>().ReverseMap();
                cfg.CreateMap<TaskInfoNotebook, NotebookModel>();

                // Map from Model
                cfg.CreateMap<ExtendedModel, TaskInfoExtendedEntry>()
                    .IncludeAllDerived()
                    .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color.ToArgb()));

                cfg.CreateMap<BaseModel, TaskInfoEntryBase>()
                    .IncludeAllDerived()
                    .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => string.Empty));

                cfg.CreateMap<NotebookModel, TaskInfoNotebook>()
                    .ForMember(dest => dest.FolderId, opt => opt.MapFrom(src => src.Folder == null ? 0 : src.Folder.Id));

                cfg.CreateMap<LocalDateTime, Instant>().ConvertUsing(s => s.InZoneLeniently(timeZone).ToInstant());
                cfg.CreateMap<Instant, LocalDateTime>().ConvertUsing(s => s.InZone(timeZone).LocalDateTime);
            });
            Mapper = config.CreateMapper();
        }
    }
}
