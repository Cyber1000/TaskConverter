using TaskConverter.Plugin.GTD.Tests.Utils;

namespace TaskConverter.Plugin.GTD.Tests.JsonTests;

public static class JsonTaskBuilderExtensions
{
    public static JsonTaskBuilder JsonTask(this IObjectBuilder _) => new();
}

public class JsonTaskBuilder
{
    private readonly Dictionary<string, object> task = [];

    public JsonTaskBuilder()
    {
        // Set default values
        task["ID"] = 8;
        task["UUID"] = "";
        task["PARENT"] = 0;
        task["CREATED"] = "2013-04-18 06:04:48.334";
        task["MODIFIED"] = "2022-05-01 10:24:01.774";
        task["TITLE"] = "Test";
        task["START_DATE"] = "";
        task["START_TIME_SET"] = 0;
        task["DUE_DATE"] = "2022-07-31 00:00";
        task["DUE_DATE_PROJECT"] = "2022-07-31 00:00";
        task["DUE_TIME_SET"] = 0;
        task["DUE_DATE_MODIFIER"] = "0";
        task["REMINDER"] = -1;
        task["ALARM"] = "";
        task["REPEAT_NEW"] = "Every 1 week";
        task["REPEAT_FROM"] = 0;
        task["DURATION"] = 0;
        task["STATUS"] = 1;
        task["CONTEXT"] = 7;
        task["GOAL"] = 0;
        task["FOLDER"] = 8;
        task["TAG"] = new[] { 8 };
        task["STARRED"] = 0;
        task["PRIORITY"] = 0;
        task["NOTE"] = "";
        task["COMPLETED"] = "";
        task["TYPE"] = 1;
        task["TRASH_BIN"] = "";
        task["IMPORTANCE"] = 0;
        task["METAINF"] = "";
        task["FLOATING"] = 0;
        task["HIDE"] = 160;
        task["HIDE_UNTIL"] = 1643583600000;
    }

    public JsonTaskBuilder WithDueDate(string dueDate)
    {
        task["DUE_DATE"] = dueDate;
        return this;
    }

    public JsonTaskBuilder WithDueDateProject(string dueDateProject)
    {
        task["DUE_DATE_PROJECT"] = dueDateProject;
        return this;
    }

    public JsonTaskBuilder WithDueTimeSet(int dueTimeSet)
    {
        task["DUE_TIME_SET"] = dueTimeSet;
        return this;
    }

    public JsonTaskBuilder WithDueDateModifier(string dueDateModifier)
    {
        task["DUE_DATE_MODIFIER"] = dueDateModifier;
        return this;
    }

    public JsonTaskBuilder WithRepeatNew(string repeatNew)
    {
        task["REPEAT_NEW"] = repeatNew;
        return this;
    }

    public JsonTaskBuilder WithReminder(long reminder)
    {
        task["REMINDER"] = reminder;
        return this;
    }

    public JsonTaskBuilder WithAlarm(string alarm)
    {
        task["ALARM"] = alarm;
        return this;
    }

    public Dictionary<string, object> Build()
    {
        return task;
    }

    public JsonTaskBuilder WithUuid(string uuid)
    {
        task["UUID"] = uuid;
        return this;
    }

    public JsonTaskBuilder WithTitle(string title)
    {
        task["TITLE"] = title;
        return this;
    }

    public JsonTaskBuilder WithStartDate(string startDate)
    {
        task["START_DATE"] = startDate;
        return this;
    }

    public JsonTaskBuilder WithStartTimeSet(bool startTimeSet)
    {
        task["START_TIME_SET"] = startTimeSet ? 1 : 0;
        return this;
    }

    public JsonTaskBuilder WithDuration(int duration)
    {
        task["DURATION"] = duration;
        return this;
    }

    public JsonTaskBuilder WithGoal(int goal)
    {
        task["GOAL"] = goal;
        return this;
    }

    public JsonTaskBuilder WithTrashBin(string trashBin)
    {
        task["TRASH_BIN"] = trashBin;
        return this;
    }

    public JsonTaskBuilder WithImportance(int importance)
    {
        task["IMPORTANCE"] = importance;
        return this;
    }

    public JsonTaskBuilder WithMetaInformation(string metaInfo)
    {
        task["METAINF"] = metaInfo;
        return this;
    }

    public JsonTaskBuilder WithHide(string hide)
    {
        task["HIDE"] = int.Parse(hide);
        return this;
    }

    public JsonTaskBuilder WithFloating(bool floating)
    {
        task["FLOATING"] = floating ? 1 : 0;
        return this;
    }
}
