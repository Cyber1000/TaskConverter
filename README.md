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

- GTD: discontinued Android-App (https://play.google.com/store/apps/details?id=com.dg.gtd.android.lite&hl=de&gl=US)
  - --from-location/--to-location: can be the original backup file named GTD*{date}*{time}.json.zip or the unzipped version
- Ical: saves to single ics-files
  - --from-location/--to-location: folder where to store/read the ics-Files

# Info for Devs

- a plugin is inherited from IConverterPlugin (for reference see TaskConverter.Plugin.GTD) and converts to/from an intermediate format in TaskConverter.Model
   - this intermediate-format is ical-format (rfc5545)
- --from-model and --from-location is bound to a plugin, converts this to ical-format and further converts this to --to-model and --to-location

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
