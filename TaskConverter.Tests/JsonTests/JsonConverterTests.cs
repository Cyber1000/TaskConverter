using System.Text.Json.JsonDiffPatch.Xunit;
using System.Xml;
using NodaTime;
using TaskConverter.Plugin.GTD;
using TaskConverter.Plugin.GTD.Model;
using TaskConverter.Plugin.GTD.Utils;
using TaskConverter.Tests.Utils;

namespace TaskConverter.Tests.JsonTests;

//TODO HH: simplify
public class JsonConverterTests
{
    [Theory]
    [InlineData(1234, 0)]
    [InlineData(5678, 1)]
    public void Convert_Tag_ShouldContainColorAndVisible(int color, int visible)
    {
        var tag = Create.A.JsonTag().WithColor(color).WithVisible(visible).Build();
        var originalJson = Create.A.JsonData().AddTag(tag).Build();

        var jsonReader = AssertJson(originalJson);
        Assert.Equal(visible == 1, jsonReader.TaskInfo?.Tag?[0].Visible);
        Assert.Equal(color, jsonReader.TaskInfo?.Tag?[0].Color);
    }

    [Fact]
    public void Convert_Task_DueDateInCorrectFormat()
    {
        var task = Create.A.JsonTask().WithDueDate("2023-12-31 00:00").WithDueDateProject("2024-01-02 00:00").WithDueTimeSet(1).WithDueDateModifier("1").Build();
        var originalJson = Create.A.JsonData().AddTask(task).Build();

        var jsonReader = AssertJson(originalJson);
        Assert.Equal(new LocalDateTime(2023, 12, 31, 0, 0), jsonReader.TaskInfo?.Task?[0].DueDate);
        Assert.Equal(DueDateModifier.DueOn, jsonReader.TaskInfo?.Task?[0].DueDateModifier);
        Assert.Equal(new LocalDateTime(2024, 1, 2, 0, 0), jsonReader.TaskInfo?.Task?[0].DueDateProject);
        Assert.Equal(true, jsonReader.TaskInfo?.Task?[0].DueTimeSet);
    }

    [Theory]
    [InlineData("Every 2 weeks", false)]
    [InlineData("Norepeat", true)]
    public void Convert_Task_RepeatNewNorepeatShouldReturnEmpty(string repeatNew, bool isNull)
    {
        var task = Create.A.JsonTask().WithRepeatNew(repeatNew).Build();
        var originalJson = Create.A.JsonData().AddTask(task).Build();

        var jsonReader = AssertJson(originalJson);
        Assert.Equal(isNull, jsonReader.TaskInfo?.Task?[0].RepeatNew == null);
    }

    [Fact]
    public void Convert_Task_ReminderValueShouldBeValid()
    {
        var task = Create.A.JsonTask().WithReminder(1510642800000).Build();
        var originalJson = Create.A.JsonData().AddTask(task).Build();

        var jsonReader = AssertJson(originalJson);
        Assert.Equal(1510642800000, jsonReader.TaskInfo?.Task?[0].Reminder);
    }

    [Fact]
    public void Convert_Task_AlarmInCorrectFormat()
    {
        var task = Create.A.JsonTask().WithAlarm("2023-12-31 00:00").Build();
        var originalJson = Create.A.JsonData().AddTask(task).Build();

        var jsonReader = AssertJson(originalJson);
        Assert.Equal(new LocalDateTime(2023, 12, 31, 0, 0), jsonReader.TaskInfo?.Task?[0].Alarm);
    }

    [Theory]
    [InlineData("Daily")]
    [InlineData("Weekly")]
    [InlineData("Biweekly")]
    [InlineData("Monthly")]
    [InlineData("Bimonthly")]
    [InlineData("Quarterly")]
    [InlineData("Semiannually")]
    [InlineData("Yearly")]
    [InlineData("Every 1 week")]
    [InlineData("Every 2 weeks")]
    [InlineData("Every 1 day")]
    public void Convert_Task_RepeatIsTranslatedCorrectly(string repeatModifier)
    {
        var task = Create.A.JsonTask().WithRepeatNew(repeatModifier).Build();
        var originalJson = Create.A.JsonData().AddTask(task).Build();

        var jsonReader = AssertJson(originalJson);
        Assert.NotNull(jsonReader.TaskInfo?.Task?[0].RepeatNew);
    }

    [Fact]
    public void Convert_Preferences_ShouldPreserveOriginalJson()
    {
        var originalJson = Create.A.JsonData().WithPreferences(CreateXmlString()).Build();

        var jsonReader = new JsonConfigurationReader(originalJson);
        var recreatedJson = jsonReader.GetJsonOutput();
        JsonAssert.Equal(JsonUtil.NormalizeText(originalJson), recreatedJson, JsonUtil.GetDiffOptions(), true);
    }

    [Fact]
    public void Convert_Preferences_ShouldPreserveXmlStructure()
    {
        var originalXmlString = CreateXmlString();
        var originalJson = Create.A.JsonData().WithPreferences(originalXmlString).Build();

        var jsonReader = AssertJson(originalJson);
        XmlDocument originalXml = new();
        originalXml.LoadXml(originalXmlString);
        var recreatedXml = jsonReader.TaskInfo!.Preferences![0].XmlConfig;

        AssertXmlEqual(originalXml, recreatedXml!);
    }

    private static string CreateXmlString()
    {
        return "<?xml version='1.0' encoding='utf-8' standalone='yes' ?>\n<map>\n    <boolean name=\"pref_show_context_label\" value=\"true\" />\n    <boolean name=\"pref_hide_until_with_time\" value=\"false\" />\n    <boolean name=\"pref_backup_before_sync\" value=\"true\" />\n    <boolean name=\"pref_show_breadcrumb\" value=\"true\" />\n    <string name=\"pref_first_day_of_week\">2</string>\n    <boolean name=\"pref_auto_hotlist_duedate\" value=\"true\" />\n    <string name=\"sync_interval\">Disabled</string>\n    <boolean name=\"pref_use_default_context_color\" value=\"true\" />\n    <boolean name=\"first_run\" value=\"false\" />\n    <boolean name=\"pref_slide_to_view_more_task_details\" value=\"false\" />\n    <string name=\"pref_vibrate\">vibrate_always</string>\n    <int name=\"fix_duplicated_folders\" value=\"18\" />\n    <boolean name=\"pref_hot_started\" value=\"false\" />\n    <boolean name=\"pref_events_missed_call_create_task\" value=\"false\" />\n    <string name=\"pref_sort_mode\">duedate</string>\n    <boolean name=\"pref_show_status\" value=\"true\" />\n    <string name=\"pref_notification_sound\">content://settings/system/notification_sound</string>\n    <boolean name=\"pref_inherit_folder\" value=\"true\" />\n    <boolean name=\"pref_capital_words_notebook\" value=\"false\" />\n    <boolean name=\"pref_inherit_tags\" value=\"true\" />\n    <boolean name=\"pref_show_tags\" value=\"true\" />\n    <boolean name=\"pref_immediate_calendar\" value=\"false\" />\n    <boolean name=\"pref_events_missed_call_create_task_with_due_date\" value=\"true\" />\n    <boolean name=\"pref_slide_to_view_more_notebook_lists\" value=\"false\" />\n    <boolean name=\"pref_hot_force_show_overdue\" value=\"true\" />\n    <boolean name=\"pref_nagging_alarm\" value=\"false\" />\n    <boolean name=\"pref_hot_star\" value=\"false\" />\n    <boolean name=\"pref_checklist_item_autom_del\" value=\"false\" />\n    <boolean name=\"pref_capital_words_task\" value=\"false\" />\n    <boolean name=\"pref_nav_toolbar\" value=\"true\" />\n    <boolean name=\"pref_capital_words_tag\" value=\"false\" />\n    <boolean name=\"pref_show_goal\" value=\"true\" />\n    <boolean name=\"pref_inherit_context\" value=\"true\" />\n    <long name=\"pref_backup_schedule\" value=\"1671328800000\" />\n    <int name=\"pref_led_color\" value=\"-65536\" />\n    <string name=\"pref_hot_priority\">-1</string>\n    <int name=\"temp_build_counters\" value=\"19\" />\n    <boolean name=\"pref_inherit_goal\" value=\"true\" />\n    <boolean name=\"pref_pure_widget_use_hotlist_filter\" value=\"false\" />\n    <boolean name=\"pref_hide_until\" value=\"false\" />\n    <int name=\"fix_too_many_files\" value=\"1\" />\n    <boolean name=\"pref_pure_widget_use_current_filter\" value=\"false\" />\n    <boolean name=\"pref_capital_words_goal\" value=\"false\" />\n    <boolean name=\"pref_enable_alarms\" value=\"true\" />\n    <int name=\"pref_dashboard_nav_view\" value=\"1\" />\n    <boolean name=\"pref_only_one_alarm\" value=\"false\" />\n    <boolean name=\"variations_seed_native_stored\" value=\"true\" />\n    <boolean name=\"pref_confirm_delete_from_list\" value=\"true\" />\n    <boolean name=\"pref_time_format\" value=\"false\" />\n    <string name=\"pref_notebook_date_format\">E, MMM dd</string>\n    <boolean name=\"pref_immediate_requery\" value=\"true\" />\n    <boolean name=\"pref_hot_nextaction\" value=\"false\" />\n    <boolean name=\"pref_show_list_label\" value=\"true\" />\n    <string name=\"CLOUDING_SERVICE\">Toodledo</string>\n    <boolean name=\"pref_pure_html_support\" value=\"false\" />\n    <boolean name=\"pref_speak_now\" value=\"true\" />\n    <boolean name=\"pref_insistent_alarm\" value=\"false\" />\n    <boolean name=\"pref_confirm_complete_task_from_list\" value=\"false\" />\n    <boolean name=\"pref_show_task_breadcrumb\" value=\"true\" />\n    <boolean name=\"pref_filter_expr_and_or\" value=\"true\" />\n    <int name=\"pref_fix_hide_until_with_date\" value=\"2\" />\n    <boolean name=\"pref_task_group\" value=\"true\" />\n    <string name=\"pref_long_date_format\">E, MMM dd, yyyy</string>\n    <boolean name=\"pref_task_list_force_show_time\" value=\"false\" />\n    <boolean name=\"pref_sort_by_completed\" value=\"false\" />\n    <boolean name=\"pref_confirm_complete_task_from_reminder\" value=\"true\" />\n    <boolean name=\"pref_task_autom_del\" value=\"false\" />\n    <string name=\"pref_hot_logical_operator\">OR</string>\n    <boolean name=\"pref_wake_screen\" value=\"false\" />\n    <boolean name=\"pref_confirm_complete_task_from_checklist\" value=\"false\" />\n    <string name=\"pref_short_date_format\">E, MMM dd</string>\n    <boolean name=\"pref_use_default_list_color\" value=\"true\" />\n    <boolean name=\"pref_task_list_show_duration\" value=\"true\" />\n    <boolean name=\"pref_hide_subtask\" value=\"false\" />\n    <boolean name=\"pref_repeating_task_remove_next_action_status\" value=\"false\" />\n    <boolean name=\"pref_auto_reminder_inbox\" value=\"false\" />\n    <string name=\"pref_widget_date_format\">dd.MM</string>\n    <int name=\"fix_goal_orphans\" value=\"1\" />\n    <boolean name=\"pref_slide_to_view_more_lists\" value=\"false\" />\n    <boolean name=\"pref_multiple_selection_status\" value=\"false\" />\n    <boolean name=\"pref_capital_words_context\" value=\"false\" />\n    <string name=\"pref_auto_reminder\">-1</string>\n    <boolean name=\"pref_search_completed_tasks\" value=\"true\" />\n    <string name=\"pref_theme\">theme_dark</string>\n    <int name=\"pref_current_version\" value=\"331\" />\n    <int name=\"pref_fix_tag_delimiter\" value=\"320\" />\n    <boolean name=\"pref_use_additional_task_notes\" value=\"true\" />\n    <boolean name=\"pref_confirm_dismiss_alarm\" value=\"true\" />\n    <int name=\"fix_null_values\" value=\"5\" />\n    <boolean name=\"pref_capital_words_folder\" value=\"false\" />\n    <boolean name=\"pref_no_clear_alarm\" value=\"false\" />\n    <boolean name=\"pref_confirm_delete_from_detail\" value=\"true\" />\n    <boolean name=\"pref_led_active\" value=\"true\" />\n    <string name=\"pref_additional_task_view_mode\">2</string>\n    <boolean name=\"pref_events_immediate_note_to_self\" value=\"true\" />\n    <int name=\"fix_vibrate_always\" value=\"2\" />\n</map>\n";
    }

    private static void AssertXmlEqual(XmlDocument expected, XmlDocument actual)
    {
        var (compareResult, xmlDiff) = XmlUtil.DiffXml(expected, actual);
        Assert.True(compareResult, $"Xml differs: {xmlDiff}");
    }

    private static JsonConfigurationReader AssertJson(string originalJson)
    {
        var jsonReader = new JsonConfigurationReader(originalJson);
        var recreatedJson = jsonReader.GetJsonOutput();
        JsonAssert.Equal(JsonUtil.NormalizeText(originalJson), recreatedJson, JsonUtil.GetDiffOptions(), true);
        return jsonReader;
    }
}
