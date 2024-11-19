using System.Xml;
using NodaTime;
using TaskConverter.Model.Model;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Tests.TestData;

public static class FindingRecordBuilderExtensions
{
    public static GTDDataModelBuilder GTDDataModel(this IObjectBuilder builder) => new GTDDataModelBuilder();
}

public class GTDDataModelBuilder
{
    private int _version;
    private readonly List<GTDFolderModel> _folderList = [];
    private readonly List<GTDContextModel> _contextList = [];
    private readonly List<GTDTagModel> _tagList = [];
    private readonly List<GTDTaskModel> _taskList = [];
    private readonly List<GTDTaskNoteModel> _taskNoteList = [];
    private readonly List<GTDNotebookModel> _notebookList = [];
    private List<GTDPreferencesModel> _preferenceList = [];

    public GTDDataModelBuilder()
    {
        WithVersion(3);
    }

    public GTDDataModelBuilder WithVersion(int version)
    {
        _version = version;
        return this;
    }

    public GTDDataModelBuilder AddDefaultFolder(int id)
    {
        _folderList.Add(
            new()
            {
                Id = id,
                Uuid = "",
                Parent = 0,
                Children = 0,
                Created = new LocalDateTime(2023, 02, 20, 10, 0, 0),
                Modified = new LocalDateTime(2023, 02, 21, 10, 0, 0),
                Title = $"Folder {id}",
                Color = -694050399,
                Visible = false,
                Ordinal = 0,
            }
        );
        return this;
    }

    public GTDDataModelBuilder AddDefaultContext(int id)
    {
        _contextList.Add(
            new()
            {
                Id = id,
                Uuid = "",
                Parent = 0,
                Children = 0,
                Created = new LocalDateTime(2023, 02, 20, 10, 0, 0),
                Modified = new LocalDateTime(2023, 02, 21, 10, 0, 0),
                Title = $"Context {id}",
                Color = -694050399,
                Visible = false,
            }
        );
        return this;
    }

    public GTDDataModelBuilder AddDefaultTag(int id)
    {
        _tagList.Add(
            new()
            {
                Id = id,
                Uuid = "",
                Created = new LocalDateTime(2023, 02, 20, 10, 0, 0),
                Modified = new LocalDateTime(2023, 02, 21, 10, 0, 0),
                Title = $"Tag {id}",
                Color = -694050399,
                Visible = false,
            }
        );
        return this;
    }

    public GTDDataModelBuilder AddDefaultTask(
        int id,
        int folderId,
        int contextId,
        List<int> tagIds,
        int parentId,
        LocalDateTime? dueDate = null,
        GTDRepeatInfoModel? repeatNew = null,
        bool? floating = null,
        long? reminder = null,
        GTDRepeatFrom? repeatFrom = null,
        Hide? hide = null,
        long? hideUntil = null
    )
    {
        dueDate ??= new LocalDateTime(2023, 02, 23, 0, 0, 0);
        repeatNew ??= new GTDRepeatInfoModel("Every 1 week");
        AddTask(id, folderId, contextId, tagIds, parentId, dueDate.Value, repeatNew.Value, floating, reminder, repeatFrom, hide, hideUntil);
        return this;
    }

    public GTDDataModelBuilder AddTask(
        int id,
        int folderId,
        int contextId,
        List<int> tagIds,
        int parentId,
        LocalDateTime? dueDate,
        GTDRepeatInfoModel? repeatNew,
        bool? floating = null,
        long? reminder = null,
        GTDRepeatFrom? repeatFrom = null,
        Hide? hide = null,
        long? hideUntil = null
    )
    {
        floating ??= false;
        reminder ??= -1;
        repeatFrom ??= GTDRepeatFrom.FromDueDate;
        hide ??= Hide.GivenDate;
        hideUntil ??= 1677402000000;

        _taskList.Add(
            new()
            {
                Id = id,
                Uuid = "",
                Parent = parentId,
                Created = new LocalDateTime(2023, 02, 20, 10, 0, 0),
                Modified = new LocalDateTime(2023, 02, 21, 10, 0, 0),
                Title = $"Task {id}",
                StartDate = null,
                StartTimeSet = false,
                DueDate = dueDate,
                DueDateProject = new LocalDateTime(2023, 02, 24, 0, 0, 0),
                DueTimeSet = false,
                DueDateModifier = DueDateModifier.DueBy,
                Reminder = reminder.Value,
                Alarm = null,
                RepeatNew = repeatNew,
                RepeatFrom = repeatFrom.Value,
                Duration = 0,
                Status = Plugin.GTD.Model.Status.NextAction,
                Context = contextId,
                Goal = 0,
                Folder = folderId,
                Tag = tagIds,
                Starred = true,
                Priority = Plugin.GTD.Model.Priority.Low,
                Note = ["Note"],
                Completed = new LocalDateTime(2023, 02, 25, 10, 0, 0),
                Type = Plugin.GTD.Model.TaskType.Task,
                TrashBin = "",
                Importance = 0,
                MetaInformation = "",
                Floating = floating.Value,
                Hide = hide.Value,
                HideUntil = hideUntil.Value,
            }
        );
        return this;
    }

    public GTDDataModelBuilder AddDefaultTaskNote(int id)
    {
        _taskNoteList.Add(
            new()
            {
                Id = id,
                Uuid = "",
                Created = new LocalDateTime(2023, 02, 20, 10, 0, 0),
                Modified = new LocalDateTime(2023, 02, 21, 10, 0, 0),
                Title = $"Task Note {id}",
                Color = -694050399,
                Visible = false,
            }
        );
        return this;
    }

    public GTDDataModelBuilder AddDefaultNotebook(int id)
    {
        _notebookList.Add(
            new()
            {
                Id = id,
                Uuid = "",
                Created = new LocalDateTime(2023, 02, 20, 10, 0, 0),
                Modified = new LocalDateTime(2023, 02, 21, 10, 0, 0),
                Private = 0,
                Title = $"Notebook {id}",
                Note = ["abc", "def"],
                FolderId = 2,
                Color = -694050399,
                Visible = false,
            }
        );
        return this;
    }

    public GTDDataModelBuilder AddPreferences(string? xmlString = null)
    {
        xmlString ??=
            "<?xml version='1.0' encoding='utf-8' standalone='yes' ?>\n<map>\n    <boolean name=\"pref_show_context_label\" value=\"true\" />\n    <boolean name=\"pref_hide_until_with_time\" value=\"false\" />\n    <boolean name=\"pref_backup_before_sync\" value=\"true\" />\n    <boolean name=\"pref_show_breadcrumb\" value=\"true\" />\n    <string name=\"pref_first_day_of_week\">2</string>\n    <boolean name=\"pref_auto_hotlist_duedate\" value=\"true\" />\n    <string name=\"sync_interval\">Disabled</string>\n    <boolean name=\"pref_use_default_context_color\" value=\"true\" />\n    <boolean name=\"first_run\" value=\"false\" />\n    <boolean name=\"pref_slide_to_view_more_task_details\" value=\"false\" />\n    <string name=\"pref_vibrate\">vibrate_always</string>\n    <int name=\"fix_duplicated_folders\" value=\"18\" />\n    <boolean name=\"pref_hot_started\" value=\"false\" />\n    <boolean name=\"pref_events_missed_call_create_task\" value=\"false\" />\n    <string name=\"pref_sort_mode\">duedate</string>\n    <boolean name=\"pref_show_status\" value=\"true\" />\n    <string name=\"pref_notification_sound\">content://settings/system/notification_sound</string>\n    <boolean name=\"pref_inherit_folder\" value=\"true\" />\n    <boolean name=\"pref_capital_words_notebook\" value=\"false\" />\n    <boolean name=\"pref_inherit_tags\" value=\"true\" />\n    <boolean name=\"pref_show_tags\" value=\"true\" />\n    <boolean name=\"pref_immediate_calendar\" value=\"false\" />\n    <boolean name=\"pref_events_missed_call_create_task_with_due_date\" value=\"true\" />\n    <boolean name=\"pref_slide_to_view_more_notebook_lists\" value=\"false\" />\n    <boolean name=\"pref_hot_force_show_overdue\" value=\"true\" />\n    <boolean name=\"pref_nagging_alarm\" value=\"false\" />\n    <boolean name=\"pref_hot_star\" value=\"false\" />\n    <boolean name=\"pref_checklist_item_autom_del\" value=\"false\" />\n    <boolean name=\"pref_capital_words_task\" value=\"false\" />\n    <boolean name=\"pref_nav_toolbar\" value=\"true\" />\n    <boolean name=\"pref_capital_words_tag\" value=\"false\" />\n    <boolean name=\"pref_show_goal\" value=\"true\" />\n    <boolean name=\"pref_inherit_context\" value=\"true\" />\n    <long name=\"pref_backup_schedule\" value=\"1671328800000\" />\n    <int name=\"pref_led_color\" value=\"-65536\" />\n    <string name=\"pref_hot_priority\">-1</string>\n    <int name=\"temp_build_counters\" value=\"19\" />\n    <boolean name=\"pref_inherit_goal\" value=\"true\" />\n    <boolean name=\"pref_pure_widget_use_hotlist_filter\" value=\"false\" />\n    <boolean name=\"pref_hide_until\" value=\"false\" />\n    <int name=\"fix_too_many_files\" value=\"1\" />\n    <boolean name=\"pref_pure_widget_use_current_filter\" value=\"false\" />\n    <boolean name=\"pref_capital_words_goal\" value=\"false\" />\n    <boolean name=\"pref_enable_alarms\" value=\"true\" />\n    <int name=\"pref_dashboard_nav_view\" value=\"1\" />\n    <boolean name=\"pref_only_one_alarm\" value=\"false\" />\n    <boolean name=\"variations_seed_native_stored\" value=\"true\" />\n    <boolean name=\"pref_confirm_delete_from_list\" value=\"true\" />\n    <boolean name=\"pref_time_format\" value=\"false\" />\n    <string name=\"pref_notebook_date_format\">E, MMM dd</string>\n    <boolean name=\"pref_immediate_requery\" value=\"true\" />\n    <boolean name=\"pref_hot_nextaction\" value=\"false\" />\n    <boolean name=\"pref_show_list_label\" value=\"true\" />\n    <string name=\"CLOUDING_SERVICE\">Toodledo</string>\n    <boolean name=\"pref_pure_html_support\" value=\"false\" />\n    <boolean name=\"pref_speak_now\" value=\"true\" />\n    <boolean name=\"pref_insistent_alarm\" value=\"false\" />\n    <boolean name=\"pref_confirm_complete_task_from_list\" value=\"false\" />\n    <boolean name=\"pref_show_task_breadcrumb\" value=\"true\" />\n    <boolean name=\"pref_filter_expr_and_or\" value=\"true\" />\n    <int name=\"pref_fix_hide_until_with_date\" value=\"2\" />\n    <boolean name=\"pref_task_group\" value=\"true\" />\n    <string name=\"pref_long_date_format\">E, MMM dd, yyyy</string>\n    <boolean name=\"pref_task_list_force_show_time\" value=\"false\" />\n    <boolean name=\"pref_sort_by_completed\" value=\"false\" />\n    <boolean name=\"pref_confirm_complete_task_from_reminder\" value=\"true\" />\n    <boolean name=\"pref_task_autom_del\" value=\"false\" />\n    <string name=\"pref_hot_logical_operator\">OR</string>\n    <boolean name=\"pref_wake_screen\" value=\"false\" />\n    <boolean name=\"pref_confirm_complete_task_from_checklist\" value=\"false\" />\n    <string name=\"pref_short_date_format\">E, MMM dd</string>\n    <boolean name=\"pref_use_default_list_color\" value=\"true\" />\n    <boolean name=\"pref_task_list_show_duration\" value=\"true\" />\n    <boolean name=\"pref_hide_subtask\" value=\"false\" />\n    <boolean name=\"pref_repeating_task_remove_next_action_status\" value=\"false\" />\n    <boolean name=\"pref_auto_reminder_inbox\" value=\"false\" />\n    <string name=\"pref_widget_date_format\">dd.MM</string>\n    <int name=\"fix_goal_orphans\" value=\"1\" />\n    <boolean name=\"pref_slide_to_view_more_lists\" value=\"false\" />\n    <boolean name=\"pref_multiple_selection_status\" value=\"false\" />\n    <boolean name=\"pref_capital_words_context\" value=\"false\" />\n    <string name=\"pref_auto_reminder\">-1</string>\n    <boolean name=\"pref_search_completed_tasks\" value=\"true\" />\n    <string name=\"pref_theme\">theme_dark</string>\n    <int name=\"pref_current_version\" value=\"331\" />\n    <int name=\"pref_fix_tag_delimiter\" value=\"320\" />\n    <boolean name=\"pref_use_additional_task_notes\" value=\"true\" />\n    <boolean name=\"pref_confirm_dismiss_alarm\" value=\"true\" />\n    <int name=\"fix_null_values\" value=\"5\" />\n    <boolean name=\"pref_capital_words_folder\" value=\"false\" />\n    <boolean name=\"pref_no_clear_alarm\" value=\"false\" />\n    <boolean name=\"pref_confirm_delete_from_detail\" value=\"true\" />\n    <boolean name=\"pref_led_active\" value=\"true\" />\n    <string name=\"pref_additional_task_view_mode\">2</string>\n    <boolean name=\"pref_events_immediate_note_to_self\" value=\"true\" />\n    <int name=\"fix_vibrate_always\" value=\"2\" />\n</map>\n";
        XmlDocument originalXml = new();
        originalXml.LoadXml(xmlString);

        _preferenceList = [new() { XmlConfig = originalXml }];
        return this;
    }

    public GTDDataModel Build()
    {
        var gtdDataModel = new GTDDataModel
        {
            Version = _version,
            Folder = ReturnNullIfListIsEmpty(_folderList),
            Context = ReturnNullIfListIsEmpty(_contextList),
            Tag = ReturnNullIfListIsEmpty(_tagList),
            Task = ReturnNullIfListIsEmpty(_taskList),
            TaskNote = ReturnNullIfListIsEmpty(_taskNoteList),
            Notebook = ReturnNullIfListIsEmpty(_notebookList),
            Preferences = ReturnNullIfListIsEmpty(_preferenceList),
        };

        return gtdDataModel;
    }

    private static List<T>? ReturnNullIfListIsEmpty<T>(List<T> list)
    {
        if (list == null || list.Count == 0)
            return null;

        return list;
    }
}
