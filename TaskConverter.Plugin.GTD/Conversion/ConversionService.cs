using System.Drawing;
using AutoMapper;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using NodaTime;
using TaskConverter.Commons;
using TaskConverter.Commons.ConversionHelper;
using TaskConverter.Commons.Utils;
using TaskConverter.Plugin.GTD.ConversionHelper;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.TodoModel;
using TaskConverter.Plugin.GTD.Utils;

namespace TaskConverter.Plugin.GTD.Conversion;

public class ConversionService : IConversionService<GTDDataModel>
{
    private IMapper Mapper { get; }

    public ISettingsProvider SettingsProvider { get; }

    public System.IO.Abstractions.IFileSystem FileSystem { get; }

    public ConversionService(IClock clock, ISettingsProvider settingsProvider, System.IO.Abstractions.IFileSystem fileSystem)
    {
        FileSystem = fileSystem;
        SettingsProvider = settingsProvider;
        var timeZone = settingsProvider.CurrentDateTimeZone;
        var config = new MapperConfiguration(cfg =>
        {
            CreateBasicMappings(cfg, timeZone);

            CreateMainMappings(clock, cfg, timeZone);
        });
        Mapper = config.CreateMapper();
    }

    public Calendar MapToIntermediateFormat(GTDDataModel taskInfo)
    {
        return Mapper.Map<Calendar>(taskInfo, opt => opt.InitializeResolutionContextForMappingToIntermediateFormat(taskInfo, SettingsProvider, FileSystem));
    }

    public GTDDataModel MapFromIntermediateFormat(Calendar model)
    {
        return Mapper.Map<GTDDataModel>(model, opt => opt.InitializeResolutionContextForMappingFromIntermediateFormat(model, SettingsProvider, FileSystem));
    }

    public void AssertConfigurationIsValid()
    {
        Mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    private static void CreateBasicMappings(IMapperConfigurationExpression cfg, DateTimeZone timeZone)
    {
        cfg.CreateMap<GTDBaseModel, UniqueComponent>()
            .ForMember(dest => dest.Uid, opt => opt.MapFrom(src => src.Id))
            .IgnoreMembers(
                dest => dest.Attendees,
                dest => dest.Comments,
                dest => dest.DtStamp,
                dest => dest.Organizer,
                dest => dest.RequestStatuses,
                dest => dest.Url,
                dest => dest.Properties,
                dest => dest.Parent,
                dest => dest.Children,
                dest => dest.Name,
                dest => dest.Calendar,
                dest => dest.Line,
                dest => dest.Column,
                dest => dest.Group
            )
            .ReverseMapWithValidation()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Uid.ToIntWithHashFallback()))
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => string.Empty))
            .IgnoreMembers(dest => dest.Created, dest => dest.Modified, dest => dest.Title!);

        cfg.CreateMap<GTDBaseModel, RecurringComponent>()
            .IgnoreMembers(
                dest => dest.Attachments,
                dest => dest.Categories,
                dest => dest.Class,
                dest => dest.Contacts,
                dest => dest.Description,
                dest => dest.DtStart,
                dest => dest.ExceptionDates,
                dest => dest.ExceptionRules,
                dest => dest.Priority,
                dest => dest.RecurrenceDates,
                dest => dest.RecurrenceRules,
                dest => dest.RecurrenceId,
                dest => dest.RelatedComponents,
                dest => dest.Sequence,
                dest => dest.Start,
                dest => dest.Alarms
            )
            .IncludeBase<GTDBaseModel, UniqueComponent>()
            .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
            .ForMember(dest => dest.LastModified, opt => opt.MapFrom(src => src.Modified))
            .ReverseMapWithValidation()
            .IncludeBase<UniqueComponent, GTDBaseModel>();

        cfg.CreateMap<GTDExtendedModel, RecurringComponent>()
            .IgnoreMembers(
                dest => dest.Attachments,
                dest => dest.Categories,
                dest => dest.Class,
                dest => dest.Contacts,
                dest => dest.Description,
                dest => dest.DtStart,
                dest => dest.ExceptionDates,
                dest => dest.ExceptionRules,
                dest => dest.Priority,
                dest => dest.RecurrenceDates,
                dest => dest.RecurrenceRules,
                dest => dest.RecurrenceId,
                dest => dest.RelatedComponents,
                dest => dest.Sequence,
                dest => dest.Start,
                dest => dest.Alarms
            )
            .IncludeBase<GTDBaseModel, RecurringComponent>()
            .AfterMap(
                (src, dest) =>
                {
                    dest.AddProperty(new CalendarProperty(IntermediateFormatPropertyNames.Color, src.Color.FromArgbWithFallback()));
                    dest.AddProperty(IntermediateFormatPropertyNames.IsVisible, src.Visible.ToStringRepresentation());
                }
            )
            .ReverseMapWithValidation()
            .IncludeBase<RecurringComponent, GTDBaseModel>()
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Properties.Get<Color?>(IntermediateFormatPropertyNames.Color).ToArgbWithFallback()))
            .ForMember(dest => dest.Visible, opt => opt.MapFrom(src => src.Properties.Get<string>(IntermediateFormatPropertyNames.IsVisible).ToBool()));

        cfg.CreateMap<LocalDateTime, IDateTime>().ConvertUsing(s => s.GetIDateTime(timeZone));
        cfg.CreateMap<LocalDateTime?, IDateTime?>().ConvertUsing(s => s.GetIDateTime(timeZone));

        cfg.CreateMap<IDateTime, LocalDateTime>().ConvertUsing(s => s.GetLocalDateTime(timeZone));
    }

    private static void CreateMainMappings(IClock clock, IMapperConfigurationExpression cfg, DateTimeZone timeZone)
    {
        cfg.CreateMap<GTDDataModel, Calendar>()
            .IgnoreMembers(
                dest => dest.UniqueComponents,
                dest => dest.RecurringItems,
                dest => dest.Events,
                dest => dest.FreeBusy,
                dest => dest.Journals,
                dest => dest.TimeZones,
                dest => dest.Todos,
                dest => dest.ProductId,
                dest => dest.Scale,
                dest => dest.Method,
                dest => dest.RecurrenceRestriction,
                dest => dest.RecurrenceEvaluationMode,
                dest => dest.Properties,
                dest => dest.Parent,
                dest => dest.Children,
                dest => dest.Name,
                dest => dest.Calendar,
                dest => dest.Line,
                dest => dest.Column,
                dest => dest.Group
            )
            .AfterMap<MapTodosToIntermediateFormat>()
            .AfterMap<MapJournalsToIntermediateFormat>()
            .AfterMap<HandlePreferencesToIntermediateFormat>()
            .ReverseMapWithValidation()
            .BeforeMap<MapKeyWordsFromIntermediateFormat>()
            .ForMember(dest => dest.Version, opt => opt.MapFrom(src => 3))
            .IgnoreMembers(
                dest => dest.Folder!,
                dest => dest.Context!,
                dest => dest.Tag!,
                dest => dest.Preferences!,
                dest => dest.GetAllEntries,
                dest => dest.TaskNote!,
                dest => dest.Notebook!,
                dest => dest.Task!
            )
            .AfterMap<MapTodoFromIntermediateFormat>()
            .AfterMap<MapJournalFromIntermediateFormat>()
            .AfterMap<HandlePreferencesFromIntermediateFormat>();

        cfg.CreateMap<GTDTaskModel, Todo>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Note != null ? src.Note.GetString() : null))
            .ForMember(dest => dest.Due, opt => opt.MapFrom(src => src.DueDate))
            .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.StartDate))
            .IgnoreMembers(
                dest => dest.DtStart,
                dest => dest.GeographicLocation,
                dest => dest.Location,
                dest => dest.PercentComplete,
                dest => dest.Resources,
                dest => dest.Attachments,
                dest => dest.Categories,
                dest => dest.Class,
                dest => dest.Contacts,
                dest => dest.ExceptionDates,
                dest => dest.ExceptionRules,
                dest => dest.RecurrenceDates,
                dest => dest.RecurrenceRules,
                dest => dest.RecurrenceId,
                dest => dest.RelatedComponents,
                dest => dest.Sequence,
                dest => dest.Status,
                dest => dest.Alarms
            )
            .AfterMap<AfterMapTodoToIntermediateFormat>()
            .AfterMap<MapKeyWordsOfTodoToIntermediateFormat>()
            .IncludeBase<GTDBaseModel, RecurringComponent>()
            .ReverseMapWithValidation()
            .ForMember(dest => dest.DueTimeSet, opt => opt.MapFrom(src => src.Due != null && src.Due.GetLocalDateTime(timeZone).TimeOfDay > new LocalTime()))
            .ForMember(dest => dest.Reminder, opt => opt.MapFrom(new MapReminderFromIntermediateFormat(timeZone)))
            .ForMember(dest => dest.Alarm, opt => opt.MapFrom(new MapAlarmFromIntermediateFormat(clock, timeZone)))
            .ForMember(dest => dest.Hide, opt => opt.MapFrom(new MapHideFromIntermediateFormat(timeZone)))
            .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Description != null ? src.Description.GetStringArray() : null))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.Properties.Get<IDateTime>(IntermediateFormatPropertyNames.Start) ?? src.Start))
            .IgnoreMembers(
                dest => dest.DueDateProject!,
                dest => dest.StartTimeSet,
                dest => dest.Duration,
                dest => dest.Context,
                dest => dest.Folder,
                dest => dest.Tag,
                dest => dest.Goal,
                dest => dest.TrashBin,
                dest => dest.Importance,
                dest => dest.MetaInformation,
                dest => dest.Parent,
                dest => dest.Type,
                dest => dest.DueDateModifier,
                dest => dest.RepeatNew!,
                dest => dest.RepeatFrom,
                dest => dest.Floating,
                dest => dest.HideUntil,
                dest => dest.Starred
            )
            .AfterMap<AfterMapTodoFromIntermediateFormat>()
            .IncludeBase<RecurringComponent, GTDBaseModel>();

        cfg.CreateMap<GTDNotebookModel, Journal>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Note != null ? src.Note.GetString() : null))
            .IgnoreMembers(
                dest => dest.Status,
                dest => dest.Attachments,
                dest => dest.Categories,
                dest => dest.Class,
                dest => dest.Contacts,
                dest => dest.DtStart,
                dest => dest.ExceptionDates,
                dest => dest.ExceptionRules,
                dest => dest.Priority,
                dest => dest.RecurrenceDates,
                dest => dest.RecurrenceRules,
                dest => dest.RecurrenceId,
                dest => dest.RelatedComponents,
                dest => dest.Sequence,
                dest => dest.Start,
                dest => dest.Alarms
            )
            .AfterMap(
                (src, dest) =>
                {
                    dest.AddProperty(IntermediateFormatPropertyNames.IsVisible, src.Visible.ToString().ToLowerInvariant());
                }
            )
            .AfterMap<MapKeyWordsOfJournalToIntermediateFormat>()
            .IncludeBase<GTDExtendedModel, RecurringComponent>()
            .ReverseMapWithValidation()
            .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Description != null ? src.Description.GetStringArray() : null))
            .IgnoreMembers(dest => dest.Private, dest => dest.FolderId)
            .ForMember(dest => dest.Visible, opt => opt.MapFrom(src => src.Properties.Get<string>(IntermediateFormatPropertyNames.IsVisible).ToBool()))
            .IncludeBase<RecurringComponent, GTDExtendedModel>();

        cfg.CreateMap<KeyWordMetaData, GTDBaseModel>()
            .IncludeAllDerived()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
            .ForMember(dest => dest.Modified, opt => opt.MapFrom(src => src.Modified))
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => string.Empty));

        cfg.CreateMap<KeyWordMetaData, GTDExtendedModel>()
            .IncludeAllDerived()
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color.ToArgbWithFallback()))
            .ForMember(dest => dest.Visible, opt => opt.MapFrom(src => src.IsVisible));

        cfg.CreateMap<KeyWordMetaData, GTDTagModel>();
        cfg.CreateMap<KeyWordMetaData, GTDFolderModel>().IgnoreMembers(dest => dest.Ordinal, dest => dest.Children, dest => dest.Parent);
        cfg.CreateMap<KeyWordMetaData, GTDContextModel>().IgnoreMembers(dest => dest.Children, dest => dest.Parent);
    }
}
