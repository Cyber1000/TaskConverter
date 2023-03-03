using System.Text.Json.JsonDiffPatch.Xunit;
using System.Xml;
using Converter.Core;
using Converter.Core.Utils;

namespace Converter.Tests;

public class JsonConverterTests
{
    [Fact]
    public void Convert_Tag_ShouldContainColorAndVisible()
    {
        var originalJson =
            " {\"version\": 3, \"TAG\": [ { \"ID\": 27, \"UUID\": \"\", \"CREATED\": \"2018-07-20 09:16:35.617\", "
            + "\"MODIFIED\": \"2018-07-20 09:16:35.617\", \"TITLE\": \"Anruf\", \"COLOR\": -1048832, \"VISIBLE\": 1 }] }";

        AssertJson(originalJson);
    }

    [Fact]
    public void Convert_Task_DueDateInCorrectFormat()
    {
        var originalJson =
            "{\"version\": 3, \"TASK\": [{\"ID\": 8, \"UUID\": \"\", \"PARENT\": 0, \"CREATED\": \"2013-04-18 06:04:48.334\", \"MODIFIED\": \"2022-05-01 10:24:01.774\", "
            + "\"TITLE\": \"Test\", \"START_DATE\": \"\", \"START_TIME_SET\": 0, \"DUE_DATE\": \"2022-07-31 00:00\", \"DUE_DATE_PROJECT\": "
            + "\"2022-07-31 00:00\", \"DUE_TIME_SET\": 0, \"DUE_DATE_MODIFIER\": \"0\", \"REMINDER\": -1, \"ALARM\": \"\", \"REPEAT_NEW\": \"Every 1 week\", "
            + "\"REPEAT_FROM\": 0, \"DURATION\": 0, \"STATUS\": 1, \"CONTEXT\": 7, \"GOAL\": 0, \"FOLDER\": 8, \"TAG\": [8], \"STARRED\": 0, "
            + "\"PRIORITY\": 0, \"NOTE\": \"\", \"COMPLETED\": \"\", \"TYPE\": 1, \"TRASH_BIN\": \"\", \"IMPORTANCE\": 0, \"METAINF\": \"\", "
            + "\"FLOATING\": 0, \"HIDE\": 160, \"HIDE_UNTIL\": 1643583600000}] }";

        AssertJson(originalJson);
    }

    [Fact]
    public void Convert_Task_RepeatNewNorepeatShouldReturnEmpty()
    {
        var originalJson =
            "{\"version\": 3, \"TASK\": [{\"ID\": 8, \"UUID\": \"\", \"PARENT\": 0, \"CREATED\": \"2013-04-18 06:04:48.334\", \"MODIFIED\": \"2022-05-01 10:24:01.774\", "
            + "\"TITLE\": \"Test\", \"START_DATE\": \"\", \"START_TIME_SET\": 0, \"DUE_DATE\": \"\", \"DUE_DATE_PROJECT\": "
            + "\"\", \"DUE_TIME_SET\": 0, \"DUE_DATE_MODIFIER\": \"0\", \"REMINDER\": -1, \"ALARM\": \"\", \"REPEAT_NEW\": \"Norepeat\", "
            + "\"REPEAT_FROM\": 0, \"DURATION\": 0, \"STATUS\": 1, \"CONTEXT\": 7, \"GOAL\": 0, \"FOLDER\": 8, \"TAG\": [8], \"STARRED\": 0, "
            + "\"PRIORITY\": 0, \"NOTE\": \"\", \"COMPLETED\": \"\", \"TYPE\": 1, \"TRASH_BIN\": \"\", \"IMPORTANCE\": 0, \"METAINF\": \"\", "
            + "\"FLOATING\": 0, \"HIDE\": 160, \"HIDE_UNTIL\": 1643583600000}] }";

        AssertJson(originalJson);
    }

    [Fact]
    public void Convert_Task_ReminderValueShouldBeValid()
    {
        var originalJson =
            "{\"version\": 3, \"TASK\": [{\"ID\": 8, \"UUID\": \"\", \"PARENT\": 0, \"CREATED\": \"2013-04-18 06:04:48.334\", \"MODIFIED\": \"2022-05-01 10:24:01.774\", "
            + "\"TITLE\": \"Test\", \"START_DATE\": \"\", \"START_TIME_SET\": 0, \"DUE_DATE\": \"\", \"DUE_DATE_PROJECT\": "
            + "\"\", \"DUE_TIME_SET\": 0, \"DUE_DATE_MODIFIER\": \"0\", \"REMINDER\": 1510642800000, \"ALARM\": \"\", \"REPEAT_NEW\": \"\", "
            + "\"REPEAT_FROM\": 0, \"DURATION\": 0, \"STATUS\": 1, \"CONTEXT\": 7, \"GOAL\": 0, \"FOLDER\": 8, \"TAG\": [8], \"STARRED\": 0, "
            + "\"PRIORITY\": 0, \"NOTE\": \"\", \"COMPLETED\": \"\", \"TYPE\": 1, \"TRASH_BIN\": \"\", \"IMPORTANCE\": 0, \"METAINF\": \"\", "
            + "\"FLOATING\": 0, \"HIDE\": 160, \"HIDE_UNTIL\": 1643583600000}] }";

        AssertJson(originalJson);
    }

    [Fact]
    public void Convert_Task_AlarmInCorrectFormat()
    {
        var originalJson =
            "{\"version\": 3, \"TASK\": [{\"ID\": 8, \"UUID\": \"\", \"PARENT\": 0, \"CREATED\": \"2013-04-18 06:04:48.334\", \"MODIFIED\": \"2022-05-01 10:24:01.774\", "
            + "\"TITLE\": \"Test\", \"START_DATE\": \"\", \"START_TIME_SET\": 0, \"DUE_DATE\": \"\", \"DUE_DATE_PROJECT\": "
            + "\"\", \"DUE_TIME_SET\": 0, \"DUE_DATE_MODIFIER\": \"0\", \"REMINDER\": -1, \"ALARM\": \"2022-07-31 00:00\", \"REPEAT_NEW\": \"\", "
            + "\"REPEAT_FROM\": 0, \"DURATION\": 0, \"STATUS\": 1, \"CONTEXT\": 7, \"GOAL\": 0, \"FOLDER\": 8, \"TAG\": [8], \"STARRED\": 0, "
            + "\"PRIORITY\": 0, \"NOTE\": \"\", \"COMPLETED\": \"\", \"TYPE\": 1, \"TRASH_BIN\": \"\", \"IMPORTANCE\": 0, \"METAINF\": \"\", "
            + "\"FLOATING\": 0, \"HIDE\": 160, \"HIDE_UNTIL\": 1643583600000}] }";

        AssertJson(originalJson);
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
        var originalJson =
            "{\"version\": 3, \"TASK\": [{\"ID\": 8, \"UUID\": \"\", \"PARENT\": 0, \"CREATED\": \"2013-04-18 06:04:48.334\", \"MODIFIED\": \"2022-05-01 10:24:01.774\", "
            + "\"TITLE\": \"Test\", \"START_DATE\": \"\", \"START_TIME_SET\": 0, \"DUE_DATE\": \"\", \"DUE_DATE_PROJECT\": "
            + "\"\", \"DUE_TIME_SET\": 0, \"DUE_DATE_MODIFIER\": \"0\", \"REMINDER\": -1, \"ALARM\": \"\", "
            + "\"REPEAT_NEW\": \""
            + repeatModifier
            + "\", "
            + "\"REPEAT_FROM\": 0, \"DURATION\": 0, \"STATUS\": 1, \"CONTEXT\": 7, \"GOAL\": 0, \"FOLDER\": 8, \"TAG\": [8], \"STARRED\": 0, "
            + "\"PRIORITY\": 0, \"NOTE\": \"\", \"COMPLETED\": \"\", \"TYPE\": 1, \"TRASH_BIN\": \"\", \"IMPORTANCE\": 0, \"METAINF\": \"\", "
            + "\"FLOATING\": 0, \"HIDE\": 160, \"HIDE_UNTIL\": 1643583600000}] }";

        AssertJson(originalJson);
    }

    [Fact]
    public void Convert_Preferences_ShouldBeFormattedCorrectly()
    {
        var originalXmlString =
            "<?xml version='1.0' encoding='utf-8' standalone='yes' ?>\n<map>\n    <boolean name=\"pref_show_context_label\" value=\"true\" />\n    <boolean name=\"pref_hide_until_with_time\" value=\"false\" />\n    <boolean name=\"pref_backup_before_sync\" value=\"true\" />\n    <boolean name=\"pref_show_breadcrumb\" value=\"true\" />\n    <string name=\"pref_first_day_of_week\">2</string>\n    <boolean name=\"pref_auto_hotlist_duedate\" value=\"true\" />\n    <string name=\"sync_interval\">Disabled</string>\n    <boolean name=\"pref_use_default_context_color\" value=\"true\" />\n    <boolean name=\"first_run\" value=\"false\" />\n    <boolean name=\"pref_slide_to_view_more_task_details\" value=\"false\" />\n    <string name=\"pref_vibrate\">vibrate_always</string>\n    <int name=\"fix_duplicated_folders\" value=\"18\" />\n    <boolean name=\"pref_hot_started\" value=\"false\" />\n    <boolean name=\"pref_events_missed_call_create_task\" value=\"false\" />\n    <string name=\"pref_sort_mode\">duedate</string>\n    <boolean name=\"pref_show_status\" value=\"true\" />\n    <string name=\"pref_notification_sound\">content://settings/system/notification_sound</string>\n    <boolean name=\"pref_inherit_folder\" value=\"true\" />\n    <boolean name=\"pref_capital_words_notebook\" value=\"false\" />\n    <boolean name=\"pref_inherit_tags\" value=\"true\" />\n    <boolean name=\"pref_show_tags\" value=\"true\" />\n    <boolean name=\"pref_immediate_calendar\" value=\"false\" />\n    <boolean name=\"pref_events_missed_call_create_task_with_due_date\" value=\"true\" />\n    <boolean name=\"pref_slide_to_view_more_notebook_lists\" value=\"false\" />\n    <boolean name=\"pref_hot_force_show_overdue\" value=\"true\" />\n    <boolean name=\"pref_nagging_alarm\" value=\"false\" />\n    <boolean name=\"pref_hot_star\" value=\"false\" />\n    <boolean name=\"pref_checklist_item_autom_del\" value=\"false\" />\n    <boolean name=\"pref_capital_words_task\" value=\"false\" />\n    <boolean name=\"pref_nav_toolbar\" value=\"true\" />\n    <boolean name=\"pref_capital_words_tag\" value=\"false\" />\n    <boolean name=\"pref_show_goal\" value=\"true\" />\n    <boolean name=\"pref_inherit_context\" value=\"true\" />\n    <long name=\"pref_backup_schedule\" value=\"1671328800000\" />\n    <int name=\"pref_led_color\" value=\"-65536\" />\n    <string name=\"pref_hot_priority\">-1</string>\n    <int name=\"temp_build_counters\" value=\"19\" />\n    <boolean name=\"pref_inherit_goal\" value=\"true\" />\n    <boolean name=\"pref_pure_widget_use_hotlist_filter\" value=\"false\" />\n    <boolean name=\"pref_hide_until\" value=\"false\" />\n    <int name=\"fix_too_many_files\" value=\"1\" />\n    <boolean name=\"pref_pure_widget_use_current_filter\" value=\"false\" />\n    <boolean name=\"pref_capital_words_goal\" value=\"false\" />\n    <boolean name=\"pref_enable_alarms\" value=\"true\" />\n    <int name=\"pref_dashboard_nav_view\" value=\"1\" />\n    <boolean name=\"pref_only_one_alarm\" value=\"false\" />\n    <boolean name=\"variations_seed_native_stored\" value=\"true\" />\n    <boolean name=\"pref_confirm_delete_from_list\" value=\"true\" />\n    <boolean name=\"pref_time_format\" value=\"false\" />\n    <string name=\"pref_notebook_date_format\">E, MMM dd</string>\n    <boolean name=\"pref_immediate_requery\" value=\"true\" />\n    <boolean name=\"pref_hot_nextaction\" value=\"false\" />\n    <boolean name=\"pref_show_list_label\" value=\"true\" />\n    <string name=\"CLOUDING_SERVICE\">Toodledo</string>\n    <boolean name=\"pref_pure_html_support\" value=\"false\" />\n    <boolean name=\"pref_speak_now\" value=\"true\" />\n    <boolean name=\"pref_insistent_alarm\" value=\"false\" />\n    <boolean name=\"pref_confirm_complete_task_from_list\" value=\"false\" />\n    <boolean name=\"pref_show_task_breadcrumb\" value=\"true\" />\n    <boolean name=\"pref_filter_expr_and_or\" value=\"true\" />\n    <int name=\"pref_fix_hide_until_with_date\" value=\"2\" />\n    <boolean name=\"pref_task_group\" value=\"true\" />\n    <string name=\"pref_long_date_format\">E, MMM dd, yyyy</string>\n    <boolean name=\"pref_task_list_force_show_time\" value=\"false\" />\n    <boolean name=\"pref_sort_by_completed\" value=\"false\" />\n    <boolean name=\"pref_confirm_complete_task_from_reminder\" value=\"true\" />\n    <boolean name=\"pref_task_autom_del\" value=\"false\" />\n    <string name=\"pref_hot_logical_operator\">OR</string>\n    <boolean name=\"pref_wake_screen\" value=\"false\" />\n    <boolean name=\"pref_confirm_complete_task_from_checklist\" value=\"false\" />\n    <string name=\"pref_short_date_format\">E, MMM dd</string>\n    <boolean name=\"pref_use_default_list_color\" value=\"true\" />\n    <boolean name=\"pref_task_list_show_duration\" value=\"true\" />\n    <boolean name=\"pref_hide_subtask\" value=\"false\" />\n    <boolean name=\"pref_repeating_task_remove_next_action_status\" value=\"false\" />\n    <boolean name=\"pref_auto_reminder_inbox\" value=\"false\" />\n    <string name=\"pref_widget_date_format\">dd.MM</string>\n    <int name=\"fix_goal_orphans\" value=\"1\" />\n    <boolean name=\"pref_slide_to_view_more_lists\" value=\"false\" />\n    <boolean name=\"pref_multiple_selection_status\" value=\"false\" />\n    <boolean name=\"pref_capital_words_context\" value=\"false\" />\n    <string name=\"pref_auto_reminder\">-1</string>\n    <boolean name=\"pref_search_completed_tasks\" value=\"true\" />\n    <string name=\"pref_theme\">theme_dark</string>\n    <int name=\"pref_current_version\" value=\"331\" />\n    <int name=\"pref_fix_tag_delimiter\" value=\"320\" />\n    <boolean name=\"pref_use_additional_task_notes\" value=\"true\" />\n    <boolean name=\"pref_confirm_dismiss_alarm\" value=\"true\" />\n    <int name=\"fix_null_values\" value=\"5\" />\n    <boolean name=\"pref_capital_words_folder\" value=\"false\" />\n    <boolean name=\"pref_no_clear_alarm\" value=\"false\" />\n    <boolean name=\"pref_confirm_delete_from_detail\" value=\"true\" />\n    <boolean name=\"pref_led_active\" value=\"true\" />\n    <string name=\"pref_additional_task_view_mode\">2</string>\n    <boolean name=\"pref_events_immediate_note_to_self\" value=\"true\" />\n    <int name=\"fix_vibrate_always\" value=\"2\" />\n</map>\n";
        var originalJson =
            $"{{\"version\": 3, \"Preferences\": [{{\"com.dg.gtd.android.lite_preferences\": \"{XmlUtil.MaskForJson(originalXmlString)}\"}}]}}";

        var jsonReader = AssertJson(originalJson);
        XmlDocument originalXml = new();
        originalXml.LoadXml(originalXmlString);
        var recreatedXml = jsonReader.TaskInfo!.Preferences![0].XmlConfig;

        var (compareResult, xmlDiff) = XmlUtil.DiffXml(originalXml, recreatedXml!);
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
