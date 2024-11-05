using NodaTime;

namespace TaskConverter.Model.Model;

public enum BaseDateOfReminderInstant
{
    FromUnixEpoch,
    FromDueDate
}

public class ReminderInstant(TaskModel taskModel,
    DateTimeZone dateTimeZone,
    BaseDateOfReminderInstant reminderInstantType,
    long millisecondsFromBaseDate
    )
{

    public BaseDateOfReminderInstant ReminderInstantType { get; } = reminderInstantType;
    public long MillisecondsFromBaseDate { get; } = millisecondsFromBaseDate;

    public Instant? AbsoluteInstant
    {
        get
        {
            if (ReminderInstantType == BaseDateOfReminderInstant.FromUnixEpoch)
                return Instant.FromUnixTimeMilliseconds(MillisecondsFromBaseDate);
            else if (taskModel.DueDate.HasValue)
            {
                var duration = Duration.FromMilliseconds(MillisecondsFromBaseDate);
                return taskModel.DueDate.Value.InZoneLeniently(dateTimeZone).ToInstant().Plus(duration);
            }
            return null;
        }
    }
}
