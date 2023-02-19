using System.Text.Json.Serialization;
using Converter.Core.GTD.ConversionHelper;

namespace Converter.Core.GTD.Model;

public enum DueDateModifier
{
    DueBy = 0,
    DueOn = 1,
    DueAfter = 2,
    OptionallyOn = 3
}

public enum RepeatFrom
{
    FromDueDate,
    FromCompletion
}

public enum Status
{
    None,
    NextAction,
    Active,
    Planning,
    Delegated,
    Waiting,
    Hold,
    Postponed,
    Someday,
    Canceled,
    Reference
}

public enum Priority
{
    Top = 3,
    High = 2,
    Med = 1,
    Low = 0,
    None = -1
}

public enum TaskType
{
    Task,
    Project,
    Checklist,
    ChecklistItem,
    Note,
    Call,
    Email,
    Sms,
    ReturnCall
}

public enum Hide
{
    DontHide = 0,
    StartDate = 5,
    DayBeforeStartDate = 6,
    WeekBeforeStartDate = 7,
    OneMonthBeforeStart = 210,
    TwoMonthsBeforeStart = 220,
    ThreeMonthsBeforeStart = 230,
    FourMonthsBeforeStart = 240,
    FiveMonthsBeforeStart = 250,
    SixMonthsBeforeStart = 260,
    TaskIsDue = 1,
    DayBeforeDue = 2,
    WeekBeforeDue = 3,
    OneMonthBeforeDue = 110,
    TwoMonthsBeforeDue = 120,
    ThreeMonthsBeforeDue = 130,
    FourMonthsBeforeDue = 140,
    FiveMonthsBeforeDue = 150,
    SixMonthsBeforeDue = 160,
    GivenDate = 4
}

public class TaskInfoTaskEntry : TaskInfoEntryBase
{
    [JsonPropertyOrder(-850)]
    public int Parent { get; set; }
    public DateTime? StartDate { get; set; }
    public bool StartTimeSet { get; set; }

    [JsonDateTimeConverter("yyyy-MM-dd HH:mm")]
    public DateTime? DueDate { get; set; }

    [JsonDateTimeConverter("yyyy-MM-dd HH:mm")]
    public DateTime? DueDateProject { get; set; }
    public bool DueTimeSet { get; set; }

    [JsonConverter(typeof(TaskInfoEnumConverter))]
    public DueDateModifier DueDateModifier { get; set; }

    //TODO HH: Reminder seems to be a unixtimestamp or minutes before DueDate
    public long Reminder { get; set; }

    [JsonDateTimeConverter("yyyy-MM-dd HH:mm")]
    public DateTime? Alarm { get; set; }

    [JsonConverter(typeof(TaskInfoJsonRepeatConverter))]
    public RepeatInfo? RepeatNew { get; set; }
    public RepeatFrom RepeatFrom { get; set; }
    public int Duration { get; set; }
    public Status Status { get; set; }
    public int Context { get; set; }
    public int Goal { get; set; }
    public int Folder { get; set; }
    public List<int> Tag { get; set; } = new List<int>();
    public bool Starred { get; set; }
    public Priority Priority { get; set; }

    [JsonConverter(typeof(TaskInfoStringArrayConverter))]
    public string[]? Note { get; set; }
    public DateTime? Completed { get; set; }
    public TaskType Type { get; set; }
    public string TrashBin { get; set; } = string.Empty;
    public int Importance { get; set; }

    [JsonPropertyName("METAINF")]
    public string MetaInformation { get; set; } = string.Empty;
    public bool Floating { get; set; }
    public Hide Hide { get; set; }

    [JsonConverter(typeof(TaskInfoUnixTimeStampConverter))]
    public DateTime? HideUntil { get; set; }
}
