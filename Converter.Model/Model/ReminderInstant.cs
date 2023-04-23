using NodaTime;

namespace Converter.Model.Model
{
    public enum BaseDateOfReminderInstant
    {
        FromUnixEpoch,
        FromDueDate
    }

    public class ReminderInstant
    {
        private readonly TaskModel taskModel;
        private readonly DateTimeZone dateTimeZone;

        public BaseDateOfReminderInstant ReminderInstantType { get; }
        public long MillisecondsFromBaseDate { get; }

        public ReminderInstant(
            TaskModel taskModel,
            DateTimeZone dateTimeZone,
            BaseDateOfReminderInstant reminderInstantType,
            long millisecondsFromBaseDate
        )
        {
            this.taskModel = taskModel;
            this.dateTimeZone = dateTimeZone;
            ReminderInstantType = reminderInstantType;
            MillisecondsFromBaseDate = millisecondsFromBaseDate;
        }

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
}
