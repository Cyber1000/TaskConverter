using System.Drawing;
using System.Xml;
using Converter.Core.GTD.InternalModel;
using Converter.Core.GTD.Model;

namespace Converter.Tests
{
    public class MappingTests
    {
        [Fact]
        public void Map_Version_ShouldBeValid()
        {
            var taskInfo = new TaskInfo { Version = 3 };

            var (taskInfoModel, taskInfoFromModel) = GetMappedInfo(taskInfo);
            var version = taskInfo.Version;
            var versionModel = taskInfoModel?.Version;
            var versionFromModel = taskInfoFromModel?.Version;

            Assert.Equal(version, versionModel);

            Assert.Equal(version, versionFromModel);
        }

        [Fact]
        public void Map_Folder_ShouldBeValid()
        {
            var taskInfo = new TaskInfo
            {
                Folder = new List<TaskInfoFolderEntry>
                {
                    new TaskInfoFolderEntry
                    {
                        Id = 1,
                        Uuid = "",
                        Parent = 0,
                        Children = 0,
                        Created = new DateTime(2023, 02, 20),
                        Modified = new DateTime(2023, 02, 21),
                        Title = "Test",
                        Color = -694050399,
                        Visible = false,
                        Ordinal = 0
                    }
                }
            };
            var (taskInfoModel, taskInfoFromModel) = GetMappedInfo(taskInfo);
            var folder = taskInfo.Folder[0];
            var folderModel = taskInfoModel?.Folders?[0]!;
            var folderFromModel = taskInfoFromModel?.Folder?[0]!;

            Assert.Equal(folder.Id, folderModel.Id);
            Assert.Equal(folder.Created, folderModel.Created);
            Assert.Equal(folder.Modified, folderModel.Modified);
            Assert.Equal(folder.Title, folderModel.Title);
            Assert.Equal(Color.FromArgb(folder.Color), folderModel.Color);
            Assert.Equal(folder.Visible, folderModel.Visible);

            Assert.Equal(folder.Id, folderFromModel.Id);
            Assert.Equal(folder.Uuid, folderFromModel.Uuid);
            Assert.Equal(folder.Parent, folderFromModel.Parent);
            Assert.Equal(folder.Children, folderFromModel.Children);
            Assert.Equal(folder.Created, folderFromModel.Created);
            Assert.Equal(folder.Modified, folderFromModel.Modified);
            Assert.Equal(folder.Title, folderFromModel.Title);
            Assert.Equal(folder.Color, folderFromModel.Color);
            Assert.Equal(folder.Visible, folderFromModel.Visible);
            Assert.Equal(folder.Ordinal, folderFromModel.Ordinal);
        }

        [Fact]
        public void Map_Context_ShouldBeValid()
        {
            var taskInfo = new TaskInfo
            {
                Context = new List<TaskInfoContextEntry>
                {
                    new TaskInfoContextEntry
                    {
                        Id = 1,
                        Uuid = "",
                        Parent = 0,
                        Children = 0,
                        Created = new DateTime(2023, 02, 20),
                        Modified = new DateTime(2023, 02, 21),
                        Title = "Test",
                        Color = -694050399,
                        Visible = false
                    }
                }
            };
            var (taskInfoModel, taskInfoFromModel) = GetMappedInfo(taskInfo);
            var context = taskInfo.Context[0];
            var contextModel = taskInfoModel?.Contexts?[0]!;
            var contextFromModel = taskInfoFromModel?.Context?[0]!;

            Assert.Equal(context.Id, contextModel.Id);
            Assert.Equal(context.Created, contextModel.Created);
            Assert.Equal(context.Modified, contextModel.Modified);
            Assert.Equal(context.Title, contextModel.Title);
            Assert.Equal(Color.FromArgb(context.Color), contextModel.Color);
            Assert.Equal(context.Visible, contextModel.Visible);

            Assert.Equal(context.Id, contextFromModel.Id);
            Assert.Equal(context.Uuid, contextFromModel.Uuid);
            Assert.Equal(context.Parent, contextFromModel.Parent);
            Assert.Equal(context.Children, contextFromModel.Children);
            Assert.Equal(context.Created, contextFromModel.Created);
            Assert.Equal(context.Modified, contextFromModel.Modified);
            Assert.Equal(context.Title, contextFromModel.Title);
            Assert.Equal(context.Color, contextFromModel.Color);
            Assert.Equal(context.Visible, contextFromModel.Visible);
        }

        [Fact]
        public void Map_Tag_ShouldBeValid()
        {
            var taskInfo = new TaskInfo
            {
                Tag = new List<TaskInfoTagEntry>
                {
                    new TaskInfoTagEntry
                    {
                        Id = 1,
                        Uuid = "",
                        Created = new DateTime(2023, 02, 20),
                        Modified = new DateTime(2023, 02, 21),
                        Title = "Test",
                        Color = -694050399,
                        Visible = false
                    }
                }
            };
            var (taskInfoModel, taskInfoFromModel) = GetMappedInfo(taskInfo);
            var tag = taskInfo.Tag[0];
            var tagModel = taskInfoModel?.Tags?[0]!;
            var tagFromModel = taskInfoFromModel?.Tag?[0]!;

            Assert.Equal(tag.Id, tagModel.Id);
            Assert.Equal(tag.Created, tagModel.Created);
            Assert.Equal(tag.Modified, tagModel.Modified);
            Assert.Equal(tag.Title, tagModel.Title);
            Assert.Equal(Color.FromArgb(tag.Color), tagModel.Color);
            Assert.Equal(tag.Visible, tagModel.Visible);

            Assert.Equal(tag.Id, tagFromModel.Id);
            Assert.Equal(tag.Uuid, tagFromModel.Uuid);
            Assert.Equal(tag.Created, tagFromModel.Created);
            Assert.Equal(tag.Modified, tagFromModel.Modified);
            Assert.Equal(tag.Title, tagFromModel.Title);
            Assert.Equal(tag.Color, tagFromModel.Color);
            Assert.Equal(tag.Visible, tagFromModel.Visible);
        }

        [Fact]
        public void Map_Task_ShouldBeValid()
        {
            var taskInfo = new TaskInfo
            {
                Task = new List<TaskInfoTaskEntry>
                {
                    new TaskInfoTaskEntry
                    {
                        Id = 2,
                        Uuid = "",
                        Parent = 1,
                        Created = new DateTime(2023, 02, 20),
                        Modified = new DateTime(2023, 02, 21),
                        Title = "Test",
                        StartDate = null,
                        StartTimeSet = false,
                        DueDate = new DateTime(2023, 02, 23),
                        DueDateProject = new DateTime(2023, 02, 24),
                        DueTimeSet = false,
                        DueDateModifier = DueDateModifier.DueBy,
                        Reminder = -1,
                        Alarm = new DateTime(2023, 02, 24),
                        RepeatNew = new RepeatInfo("every 1 week"),
                        RepeatFrom = RepeatFrom.FromDueDate,
                        Duration = 0,
                        Status = Status.NextAction,
                        Context = 5,
                        Goal = 0,
                        Folder = 8,
                        Tag = new List<int> { 10, 11 },
                        Starred = true,
                        Priority = Priority.Low,
                        Note = new string[] { "Note" },
                        Completed = new DateTime(2023, 02, 25),
                        Type = TaskType.Task,
                        TrashBin = "",
                        Importance = 0,
                        MetaInformation = "",
                        Floating = true,
                        Hide = Hide.DontHide,
                        HideUntil = new DateTime(2023, 02, 26)
                    },
                    new TaskInfoTaskEntry
                    {
                        Id = 1,
                        Uuid = "",
                        Parent = 0,
                        Created = new DateTime(2023, 02, 20),
                        Modified = new DateTime(2023, 02, 21),
                        Title = "Test"
                    }
                },
                Context = new List<TaskInfoContextEntry>
                {
                    new TaskInfoContextEntry
                    {
                        Id = 5,
                        Uuid = "",
                        Parent = 0,
                        Children = 0,
                        Created = new DateTime(2023, 02, 20),
                        Modified = new DateTime(2023, 02, 21),
                        Title = "Context",
                        Color = -694050399,
                        Visible = false
                    }
                },
                Folder = new List<TaskInfoFolderEntry>
                {
                    new TaskInfoFolderEntry
                    {
                        Id = 8,
                        Uuid = "",
                        Parent = 0,
                        Children = 0,
                        Created = new DateTime(2023, 02, 20),
                        Modified = new DateTime(2023, 02, 21),
                        Title = "Folder",
                        Color = -694050399,
                        Visible = false,
                        Ordinal = 0
                    }
                },
                Tag = new List<TaskInfoTagEntry>
                {
                    new TaskInfoTagEntry
                    {
                        Id = 10,
                        Uuid = "",
                        Created = new DateTime(2023, 02, 20),
                        Modified = new DateTime(2023, 02, 21),
                        Title = "Tag1",
                        Color = -694050399,
                        Visible = false
                    },
                    new TaskInfoTagEntry
                    {
                        Id = 11,
                        Uuid = "",
                        Created = new DateTime(2023, 02, 20),
                        Modified = new DateTime(2023, 02, 21),
                        Title = "Tag2",
                        Color = -694050399,
                        Visible = false
                    }
                }
            };
            var (taskInfoModel, taskInfoFromModel) = GetMappedInfo(taskInfo);
            var task = taskInfo.Task[0];
            var taskModel = taskInfoModel?.Tasks?[0]!;
            var taskModelWithoutParent = taskInfoModel?.Tasks?[1]!;

            var taskFromModel = taskInfoFromModel?.Task?[0]!;
            var taskFromModelWithoutParent = taskInfoFromModel?.Task?[1]!;

            Assert.Equal(task.Id, taskModel.Id);
            Assert.Equal(task.Parent, taskModel.Parent!.Id);
            Assert.Equal(task.Created, taskModel.Created);
            Assert.Equal(task.Modified, taskModel.Modified);
            Assert.Equal(task.Title, taskModel.Title);
            Assert.Equal(task.DueDate, taskModel.DueDate);
            Assert.Equal(task.DueDateProject, taskModel.DueDateProject);
            Assert.Equal(task.DueTimeSet, taskModel.DueTimeSet);
            Assert.Equal(task.DueDateModifier, taskModel.DueDateModifier);
            Assert.Equal(task.Reminder, taskModel.Reminder);
            Assert.Equal(task.Alarm, taskModel.Alarm);
            Assert.Equal(task.RepeatNew?.Interval, taskModel.RepeatInfo?.Interval);
            Assert.Equal(task.RepeatNew?.Period, taskModel.RepeatInfo?.Period);
            Assert.Equal(task.RepeatFrom, taskModel.RepeatInfo?.RepeatFrom);
            Assert.Equal(task.Status, taskModel.Status);
            Assert.Equal(task.Context, taskModel.Context!.Id);
            Assert.Equal(task.Folder, taskModel.Folder!.Id);
            Assert.Equal(task.Tag, taskModel.Tags.Select(t => t.Id));
            Assert.Equal(task.Starred, taskModel.Starred);
            Assert.Equal(task.Priority, taskModel.Priority);
            Assert.Equal(task.Note, taskModel.Note);
            Assert.Equal(task.Completed, taskModel.Completed);
            Assert.Equal(task.Type, taskModel.Type);
            Assert.Equal(task.Floating, taskModel.Floating);
            Assert.Equal(task.Hide, taskModel.Hide);
            Assert.Equal(task.HideUntil, taskModel.HideUntil);

            Assert.Null(taskModelWithoutParent.Parent);
            Assert.Equal(0, taskFromModelWithoutParent.Parent);

            Assert.Equal(task.Id, taskFromModel.Id);
            Assert.Equal(task.Uuid, taskFromModel.Uuid);
            Assert.Equal(task.Parent, taskFromModel.Parent);
            Assert.Equal(task.Created, taskFromModel.Created);
            Assert.Equal(task.Modified, taskFromModel.Modified);
            Assert.Equal(task.Title, taskFromModel.Title);
            Assert.Null(taskFromModel.StartDate);
            Assert.False(taskFromModel.StartTimeSet);
            Assert.Equal(task.DueDate, taskFromModel.DueDate);
            Assert.Equal(task.DueDateProject, taskFromModel.DueDateProject);
            Assert.Equal(task.DueTimeSet, taskFromModel.DueTimeSet);
            Assert.Equal(task.DueDateModifier, taskFromModel.DueDateModifier);
            Assert.Equal(task.Reminder, taskFromModel.Reminder);
            Assert.Equal(task.Alarm, taskFromModel.Alarm);
            Assert.Equal(task.RepeatNew?.Interval, taskFromModel.RepeatNew?.Interval);
            Assert.Equal(task.RepeatNew?.Period, taskFromModel.RepeatNew?.Period);
            Assert.Equal(task.RepeatFrom, taskFromModel.RepeatFrom);
            Assert.Equal(0, taskFromModel.Duration);
            Assert.Equal(task.Status, taskFromModel.Status);
            Assert.Equal(task.Context, taskFromModel.Context);
            Assert.Equal(0, taskFromModel.Goal);
            Assert.Equal(task.Folder, taskFromModel.Folder);
            Assert.Equal(task.Tag, taskFromModel.Tag);
            Assert.Equal(task.Starred, taskFromModel.Starred);
            Assert.Equal(task.Priority, taskFromModel.Priority);
            Assert.Equal(task.Note, taskFromModel.Note);
            Assert.Equal(task.Completed, taskFromModel.Completed);
            Assert.Equal(task.Type, taskFromModel.Type);
            Assert.Empty(taskFromModel.TrashBin);
            Assert.Equal(0, taskFromModel.Importance);
            Assert.Empty(taskFromModel.MetaInformation);
            Assert.Equal(task.Floating, taskFromModel.Floating);
            Assert.Equal(task.Hide, taskFromModel.Hide);
            Assert.Equal(task.HideUntil, taskFromModel.HideUntil);
        }

        [Fact]
        public void Map_TaskNote_ShouldBeValid()
        {
            var taskInfo = new TaskInfo
            {
                TaskNote = new List<TaskInfoTaskNote>
                {
                    new TaskInfoTaskNote
                    {
                        Id = 1,
                        Uuid = "",
                        Created = new DateTime(2023, 02, 20),
                        Modified = new DateTime(2023, 02, 21),
                        Title = "Test",
                        Color = -694050399,
                        Visible = false
                    }
                }
            };
            var (taskInfoModel, taskInfoFromModel) = GetMappedInfo(taskInfo);
            var taskNote = taskInfo.TaskNote[0];
            var taskNoteModel = taskInfoModel?.TaskNotes?[0]!;
            var taskNoteFromModel = taskInfoFromModel?.TaskNote?[0]!;

            Assert.Equal(taskNote.Id, taskNoteModel.Id);
            Assert.Equal(taskNote.Created, taskNoteModel.Created);
            Assert.Equal(taskNote.Modified, taskNoteModel.Modified);
            Assert.Equal(taskNote.Title, taskNoteModel.Title);
            Assert.Equal(Color.FromArgb(taskNote.Color), taskNoteModel.Color);
            Assert.Equal(taskNote.Visible, taskNoteModel.Visible);

            Assert.Equal(taskNote.Id, taskNoteFromModel.Id);
            Assert.Equal(taskNote.Uuid, taskNoteFromModel.Uuid);
            Assert.Equal(taskNote.Created, taskNoteFromModel.Created);
            Assert.Equal(taskNote.Modified, taskNoteFromModel.Modified);
            Assert.Equal(taskNote.Title, taskNoteFromModel.Title);
            Assert.Equal(taskNote.Color, taskNoteFromModel.Color);
            Assert.Equal(taskNote.Visible, taskNoteFromModel.Visible);
        }

        [Fact]
        public void Map_Notebook_ShouldBeValid()
        {
            var taskInfo = new TaskInfo
            {
                Notebook = new List<TaskInfoNotebook>
                {
                    new TaskInfoNotebook
                    {
                        Id = 1,
                        Uuid = "",
                        Created = new DateTime(2023, 02, 20),
                        Modified = new DateTime(2023, 02, 21),
                        Private = 0,
                        Title = "Test",
                        Note = new string[] { "abc", "def" },
                        FolderId = 2,
                        Color = -694050399,
                        Visible = false
                    }
                },
                Folder = new List<TaskInfoFolderEntry>
                {
                    new TaskInfoFolderEntry
                    {
                        Id = 2,
                        Uuid = "",
                        Parent = 0,
                        Children = 0,
                        Created = new DateTime(2023, 02, 20),
                        Modified = new DateTime(2023, 02, 21),
                        Title = "Folder",
                        Color = -694050399,
                        Visible = false,
                        Ordinal = 0
                    }
                }
            };
            var (taskInfoModel, taskInfoFromModel) = GetMappedInfo(taskInfo);
            var notebook = taskInfo.Notebook[0];
            var notebookModel = taskInfoModel?.Notebooks?[0]!;
            var notebookFromModel = taskInfoFromModel?.Notebook?[0]!;

            Assert.Equal(notebook.Id, notebookModel.Id);
            Assert.Equal(notebook.Created, notebookModel.Created);
            Assert.Equal(notebook.Modified, notebookModel.Modified);
            Assert.Equal(notebook.Title, notebookModel.Title);
            Assert.Equal(notebook.Note, notebookModel.Note);
            Assert.Equal(notebook.FolderId, notebookModel.Folder?.Id);
            Assert.Equal(Color.FromArgb(notebook.Color), notebookModel.Color);
            Assert.Equal(notebook.Visible, notebookModel.Visible);

            Assert.Equal(notebook.Id, notebookFromModel.Id);
            Assert.Equal(notebook.Uuid, notebookFromModel.Uuid);
            Assert.Equal(notebook.Created, notebookFromModel.Created);
            Assert.Equal(notebook.Modified, notebookFromModel.Modified);
            Assert.Equal(notebook.Private, notebookFromModel.Private);
            Assert.Equal(notebook.Title, notebookFromModel.Title);
            Assert.Equal(notebook.Note, notebookFromModel.Note);
            Assert.Equal(notebook.FolderId, notebookFromModel.FolderId);
            Assert.Equal(notebook.Color, notebookFromModel.Color);
            Assert.Equal(notebook.Visible, notebookFromModel.Visible);
        }

        [Fact]
        public void Map_Preferences_ShouldBeValid()
        {
            var originalXmlString =
                "<?xml version='1.0' encoding='utf-8' standalone='yes' ?>\n<map>\n    <boolean name=\"pref_show_context_label\" value=\"true\" />\n    <boolean name=\"pref_hide_until_with_time\" value=\"false\" />\n    <boolean name=\"pref_backup_before_sync\" value=\"true\" />\n    <boolean name=\"pref_show_breadcrumb\" value=\"true\" />\n    <string name=\"pref_first_day_of_week\">2</string>\n    <boolean name=\"pref_auto_hotlist_duedate\" value=\"true\" />\n    <string name=\"sync_interval\">Disabled</string>\n    <boolean name=\"pref_use_default_context_color\" value=\"true\" />\n    <boolean name=\"first_run\" value=\"false\" />\n    <boolean name=\"pref_slide_to_view_more_task_details\" value=\"false\" />\n    <string name=\"pref_vibrate\">vibrate_always</string>\n    <int name=\"fix_duplicated_folders\" value=\"18\" />\n    <boolean name=\"pref_hot_started\" value=\"false\" />\n    <boolean name=\"pref_events_missed_call_create_task\" value=\"false\" />\n    <string name=\"pref_sort_mode\">duedate</string>\n    <boolean name=\"pref_show_status\" value=\"true\" />\n    <string name=\"pref_notification_sound\">content://settings/system/notification_sound</string>\n    <boolean name=\"pref_inherit_folder\" value=\"true\" />\n    <boolean name=\"pref_capital_words_notebook\" value=\"false\" />\n    <boolean name=\"pref_inherit_tags\" value=\"true\" />\n    <boolean name=\"pref_show_tags\" value=\"true\" />\n    <boolean name=\"pref_immediate_calendar\" value=\"false\" />\n    <boolean name=\"pref_events_missed_call_create_task_with_due_date\" value=\"true\" />\n    <boolean name=\"pref_slide_to_view_more_notebook_lists\" value=\"false\" />\n    <boolean name=\"pref_hot_force_show_overdue\" value=\"true\" />\n    <boolean name=\"pref_nagging_alarm\" value=\"false\" />\n    <boolean name=\"pref_hot_star\" value=\"false\" />\n    <boolean name=\"pref_checklist_item_autom_del\" value=\"false\" />\n    <boolean name=\"pref_capital_words_task\" value=\"false\" />\n    <boolean name=\"pref_nav_toolbar\" value=\"true\" />\n    <boolean name=\"pref_capital_words_tag\" value=\"false\" />\n    <boolean name=\"pref_show_goal\" value=\"true\" />\n    <boolean name=\"pref_inherit_context\" value=\"true\" />\n    <long name=\"pref_backup_schedule\" value=\"1671328800000\" />\n    <int name=\"pref_led_color\" value=\"-65536\" />\n    <string name=\"pref_hot_priority\">-1</string>\n    <int name=\"temp_build_counters\" value=\"19\" />\n    <boolean name=\"pref_inherit_goal\" value=\"true\" />\n    <boolean name=\"pref_pure_widget_use_hotlist_filter\" value=\"false\" />\n    <boolean name=\"pref_hide_until\" value=\"false\" />\n    <int name=\"fix_too_many_files\" value=\"1\" />\n    <boolean name=\"pref_pure_widget_use_current_filter\" value=\"false\" />\n    <boolean name=\"pref_capital_words_goal\" value=\"false\" />\n    <boolean name=\"pref_enable_alarms\" value=\"true\" />\n    <int name=\"pref_dashboard_nav_view\" value=\"1\" />\n    <boolean name=\"pref_only_one_alarm\" value=\"false\" />\n    <boolean name=\"variations_seed_native_stored\" value=\"true\" />\n    <boolean name=\"pref_confirm_delete_from_list\" value=\"true\" />\n    <boolean name=\"pref_time_format\" value=\"false\" />\n    <string name=\"pref_notebook_date_format\">E, MMM dd</string>\n    <boolean name=\"pref_immediate_requery\" value=\"true\" />\n    <boolean name=\"pref_hot_nextaction\" value=\"false\" />\n    <boolean name=\"pref_show_list_label\" value=\"true\" />\n    <string name=\"CLOUDING_SERVICE\">Toodledo</string>\n    <boolean name=\"pref_pure_html_support\" value=\"false\" />\n    <boolean name=\"pref_speak_now\" value=\"true\" />\n    <boolean name=\"pref_insistent_alarm\" value=\"false\" />\n    <boolean name=\"pref_confirm_complete_task_from_list\" value=\"false\" />\n    <boolean name=\"pref_show_task_breadcrumb\" value=\"true\" />\n    <boolean name=\"pref_filter_expr_and_or\" value=\"true\" />\n    <int name=\"pref_fix_hide_until_with_date\" value=\"2\" />\n    <boolean name=\"pref_task_group\" value=\"true\" />\n    <string name=\"pref_long_date_format\">E, MMM dd, yyyy</string>\n    <boolean name=\"pref_task_list_force_show_time\" value=\"false\" />\n    <boolean name=\"pref_sort_by_completed\" value=\"false\" />\n    <boolean name=\"pref_confirm_complete_task_from_reminder\" value=\"true\" />\n    <boolean name=\"pref_task_autom_del\" value=\"false\" />\n    <string name=\"pref_hot_logical_operator\">OR</string>\n    <boolean name=\"pref_wake_screen\" value=\"false\" />\n    <boolean name=\"pref_confirm_complete_task_from_checklist\" value=\"false\" />\n    <string name=\"pref_short_date_format\">E, MMM dd</string>\n    <boolean name=\"pref_use_default_list_color\" value=\"true\" />\n    <boolean name=\"pref_task_list_show_duration\" value=\"true\" />\n    <boolean name=\"pref_hide_subtask\" value=\"false\" />\n    <boolean name=\"pref_repeating_task_remove_next_action_status\" value=\"false\" />\n    <boolean name=\"pref_auto_reminder_inbox\" value=\"false\" />\n    <string name=\"pref_widget_date_format\">dd.MM</string>\n    <int name=\"fix_goal_orphans\" value=\"1\" />\n    <boolean name=\"pref_slide_to_view_more_lists\" value=\"false\" />\n    <boolean name=\"pref_multiple_selection_status\" value=\"false\" />\n    <boolean name=\"pref_capital_words_context\" value=\"false\" />\n    <string name=\"pref_auto_reminder\">-1</string>\n    <boolean name=\"pref_search_completed_tasks\" value=\"true\" />\n    <string name=\"pref_theme\">theme_dark</string>\n    <int name=\"pref_current_version\" value=\"331\" />\n    <int name=\"pref_fix_tag_delimiter\" value=\"320\" />\n    <boolean name=\"pref_use_additional_task_notes\" value=\"true\" />\n    <boolean name=\"pref_confirm_dismiss_alarm\" value=\"true\" />\n    <int name=\"fix_null_values\" value=\"5\" />\n    <boolean name=\"pref_capital_words_folder\" value=\"false\" />\n    <boolean name=\"pref_no_clear_alarm\" value=\"false\" />\n    <boolean name=\"pref_confirm_delete_from_detail\" value=\"true\" />\n    <boolean name=\"pref_led_active\" value=\"true\" />\n    <string name=\"pref_additional_task_view_mode\">2</string>\n    <boolean name=\"pref_events_immediate_note_to_self\" value=\"true\" />\n    <int name=\"fix_vibrate_always\" value=\"2\" />\n</map>\n";
            XmlDocument originalXml = new();
            originalXml.LoadXml(originalXmlString);

            var taskInfo = new TaskInfo { Preferences = new List<Preferences>() { new Preferences { XmlConfig = originalXml } } };
            var (taskInfoModel, taskInfoFromModel) = GetMappedInfo(taskInfo);
            var preference = taskInfo.Preferences[0].XmlConfig;
            var preferenceModel = taskInfoModel?.Config;
            var preferenceFromModel = taskInfoFromModel?.Preferences?[0]?.XmlConfig;

            Assert.Equal(preferenceModel, preference);

            Assert.Equal(preferenceFromModel, preference);
        }

        private static (TaskInfoModel? model, TaskInfo? fromModel) GetMappedInfo(TaskInfo taskInfo)
        {
            if (taskInfo == null)
                return (null, null);
            var taskInfoModel = Core.Mapper.Converter.MapToModel(taskInfo);
            var taskInfoFromModel = Core.Mapper.Converter.MapFromModel(taskInfoModel);

            return (taskInfoModel, taskInfoFromModel);
        }
    }
}
