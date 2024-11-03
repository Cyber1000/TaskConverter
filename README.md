# :warning: Announcement: PLEASE READ :warning:

- Work in progress, for now I'm only able to convert GTD (an uncontinued android app) to an intermediate format
  - planned: convert to different task apps and/or csvs

# About TaskConverter

- ![Tests](https://github.com/cyber1000/TaskConverter/workflows/Build%20and%20Test/badge.svg)
- There are many (FOSS) task/project management - apps around
- The core of these apps is mostly quite similar: Having a task with notes, attachments and sometimes they are nested deeper within projects, epics, ...
- Purpose of this converter is to convert between different task/project-management-apps
- this may be a one time conversion or a regulary
  sync

# Usage

- Get information, which plugins can be used: TaskConverter.Console.exe
- Get help on available parameters: TaskConverter.Console.exe --help
- Parameters:
  - --command-type:
    - CheckSource: checks if the source is valid
    - CanMap: checks if source can be mapped to destination
  - --from-model {pluginname}: name of plugin where to convert from
  - --from-location {location}: location can be a (REST-)URL or a filename, depending on the used from-model

## Examples

- Check if your source is valid or something neeeds to be done at the example of gtd: TaskConverter.Console.exe --command-type CheckSource --from-model gtd --from-location /home/workspace/openproject/GTD_20221217_030000432.json.zip
- Check if your source can be mapped to the intermediate format at the example of gtd: TaskConverter.Console.exe --command-type CanMap --from-model gtd --from-location /home/workspace/openproject/GTD_20221217_030000432.json.zip

## Current Plugins

- GTD: discontinued Android-App (https://play.google.com/store/apps/details?id=com.dg.gtd.android.lite&hl=de&gl=US)
  - --from-location: can be the original backup file named GTD*{date}*{time}.json.zip or the unzipped version

# Info for Devs

- a plugin is inherited from IConverterPlugin (for reference see TaskConverter.Plugin.GTD) and converts to/from an intermediate format in TaskConverter.Model
- for now the console only supports a --from-model and --from-location, a --to-model and --to-location will follow
- so the plan is following path: todo-app-1 -> plugin for todo-app-1 converts to -> intermediate format -> plugin for todo-app-2 converts to -> todo-app-2

# 3rd Party - Licenses

- https://github.com/nodatime/nodatime.org: Apache-2.0
- https://github.com/dotnet/command-line-api: MIT
- https://github.com/natemcmaster/DotNetCorePlugins: Apache-2.0
- https://github.com/weichch/system-text-json-jsondiffpatch: MIT
- https://github.com/xunit/xunit: Apache-2.0

See more 3rd-party licences in individual plugins, named TaskConverter.Plugin.{PluginName}
