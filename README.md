# About TaskConverter

- There are many (FOSS) task/project management - apps around
- The core of these apps is mostly quite similar: Having a task with notes, attachments and sometimes they are nested deeper within projects, epics, ...
- Purpose of this converter is to convert between different task/project-management-apps
- this may be a one time conversion or a regulary sync

# Usage

- Get information, which plugins can be used: TaskConverter.Console.exe
- Get help on available parameters: TaskConverter.Console.exe --help
- Parameters:
  - --command-type:
    - CheckSource: checks if the source is valid
    - CanMap: checks if source can be mapped to destination
    - Map: maps from one plugin to another
  - --from-model {pluginname}: name of plugin where to convert from
  - --from-location {location}: location can be a (REST-)URL or a filename, depending on the used from-model
  - --to-model {pluginname}: name of plugin where to convert to
  - --to-location {location}: location can be a (REST-)URL or a filename, depending on the used to-model

## Examples

- Check if your source is valid or something needs to be done at the example of gtd: TaskConverter.Console.exe --command-type CheckSource --from-model gtd --from-location /home/workspace/openproject/GTD_20221217_030000432.json.zip
- Check if your source can be mapped to the intermediate format at the example of gtd: TaskConverter.Console.exe --command-type CanMap --from-model gtd --from-location /home/workspace/openproject/GTD_20221217_030000432.json.zip
- Map from gtd to single ical-files (for example for radicale server): TaskConverter.Console.exe --command-type Map --from-model gtd --from-location /home/workspace/openproject/GTD_20221217_030000432.json.zip --to-model ical --to-location /home/workspace/openproject/ical

## Current Plugins

- GTD: DGT GTD, an Android app (https://play.google.com/store/apps/details?id=com.dg.gtd.android.lite&hl=de&gl=US)
  - --from-location/--to-location: can be the original backup file named GTD*{date}*{time}.json.zip or the unzipped version
  - not supported: encrypted backups (`*.json.zip.enc`), the `GOAL` section, and four repeat modes
    - `With parent`, `Every <weekday>`, `The <n>th <weekday> of each month` and
      `Last day of every <n> months` have no iCalendar equivalent and are refused by name
  - `--to-location` writes a zip archive when it ends in `.zip`, plain JSON otherwise - the same
    two forms the reader accepts
- Ical: saves to single ics-files
  - --from-location/--to-location: folder where to store/read the ics-Files
  - on a repeated run, files an earlier run wrote that are no longer in the source are reported.
    Set `Ical.DeleteOrphanedFiles` to `true` to have them removed instead. Only files carrying this
    converter's `PRODID` are ever considered, so anything else in the destination is left alone.

# Info for Devs

- a plugin is inherited from IConverterPlugin (for reference see TaskConverter.Plugin.GTD) and converts to/from an intermediate format in TaskConverter.Model
   - this intermediate-format is ical-format (rfc5545)
- --from-model and --from-location is bound to a plugin, converts this to ical-format and further converts this to --to-model and --to-location

## The intermediate format

The intermediate format is a lossless transport between plugins, not a calendar meant to look
pretty in a foreign client. Where RFC 5545 cannot carry a field, an `X-DGT-*` property does, and
the value carried always wins over anything derived from the standard properties. A calendar
written elsewhere has none of them, so every reader falls back to plain iCalendar - that fallback
is the only reason the derivations still exist.

**Identity.** `UID` is `<type>-<id>`, e.g. `task-2970` or `notebook-5`. GTD ids are unique per
entity type only, and the Ical plugin names its files after the UID, so an untyped id let a
notebook overwrite a task. Parent links use `RELATED-TO;RELTYPE=PARENT` with the same scheme. A
UID that does not match the scheme is hashed into an id, which keeps foreign calendars readable.

**Keywords.** Folders, contexts, tags and statuses all become `CATEGORIES` entries, distinguished
by a configurable symbol (`+` folder, `@` context, `#` status by default, tags carry none). The
symbol differs between the two sides: `GTD.GTDFormat.Symbol.*` is what the app stores,
`GTD.IntermediateFormat.Symbol.*` what the calendar carries. Everything the category itself cannot
express - the original id, colour, timestamps, visibility - travels in one
`X-DGT-CATEGORY-<Type>-<Name>` property per keyword. Property names are matched case
insensitively, because a parser is free to return them uppercased.

**Carried per task**

| Property | Carries |
|---|---|
| `X-DGT-START` | start date; written even when empty, so an absent property means a foreign calendar and `DTSTART` is used instead. A repeating task has a `DTSTART` as the base of its recurrence, which is not a start date. |
| `X-DGT-ALARM` | alarm; a task can have both an alarm and a reminder, and they share the single `VALARM` |
| `X-DGT-HIDE`, `X-DGT-HIDE-UNTIL` | hide mode and hide date |
| `X-DGT-DUE-DATE-MODIFIER`, `X-DGT-DUE-TIME-SET`, `X-DGT-DUE-FLOAT`, `X-DGT-DUE-DATE-PROJECT` | the due date details; modifier and floating are independent of each other |
| `X-DGT-REPEAT-FROM` | whether a repetition counts from the due date or from completion |
| `X-DGT-TASK-TYPE` | task, project or checklist |
| `X-DGT-STARRED` | starred flag |
| `X-DGT-PRIORITY` | the exact GTD priority, since `Low` and `None` share the standard's 0 |
| `X-DGT-CREATED-MS`, `X-DGT-MODIFIED-MS`, `X-DGT-COMPLETED-MS` | the millisecond fraction of the respective timestamp, since RFC 5545 stores whole seconds only |
| `X-DGT-COLOR`, `X-DGT-ISVISIBLE` | colour and visibility of keywords and notebooks |

`PRIORITY` uses the standard property, but not naively. GTD's `Low` is the default of its enum and
therefore means "never touched" rather than "low" - in a real backup 5144 of 5998 tasks carry it, and
mapping those onto 7 made every one of them appear as an explicitly low priority task in the target
app. `Low` and `None` both become 0 ("no priority"), `Med` 5, `High` 3, `Top` 1. That makes the two
indistinguishable in the standard property, which is why `X-DGT-PRIORITY` carries the exact value.
Reading accepts the full range 0 to 9 that a foreign client may write.

**Repetitions** map onto `RRULE`. An interval over a period becomes `FREQ`/`INTERVAL`, and the two
weekday-set modes become a weekly rule with `BYDAY` (`MO,TU,WE,TH,FR` and `SA,SU`). A mode with no
iCalendar equivalent is refused with a message naming it, rather than silently becoming something
else - which is what used to happen to `Last day of every <n> months`, because the pattern matching
the interval form was not anchored and matched the substring.

**Known differences after a full roundtrip.** A repetition comes back in one spelling
(`Norepeat` as an empty string, `Monthly` as `Every 1 month`) while the app writes both forms for
the same meaning, so there is nothing to normalise towards: measured on a real backup, preferring
the named form repairs 6 tasks and breaks 30. The values are equivalent to the app, but a
`CheckSource` on the converted file shows them as a diff. Line endings inside notes are normalised
to `\n`. A keyword that no task or notebook references has no category to live in and therefore
disappears.

# 3rd Party - Licenses

- https://github.com/ical-org/ical.net: MIT
- https://github.com/natemcmaster/DotNetCorePlugins: Apache-2.0
- https://github.com/dotnet/command-line-api: MIT
- https://github.com/nodatime/nodatime.org: Apache-2.0
- https://github.com/TestableIO/System.IO.Abstractions: MIT
- https://github.com/ical-org/ical.net: MIT
- https://github.com/weichch/system-text-json-jsondiffpatch: MIT
- https://github.com/xunit/xunit: Apache-2.0

See more 3rd-party licences in individual plugins, named TaskConverter.Plugin.{PluginName}
