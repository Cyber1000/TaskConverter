using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Utils;

public static class MapStatusExtension
{
    private static readonly Dictionary<Status, string> intermediateStateMapper = new()
    {
        { Status.None, "NEEDS-ACTION" },
        { Status.NextAction, "IN-PROCESS" },
        { Status.Active, "IN-PROCESS" },
        { Status.Planning, "IN-PROCESS" },
        { Status.Delegated, "IN-PROCESS" },
        { Status.Waiting, "NEEDS-ACTION" },
        { Status.Hold, "NEEDS-ACTION" },
        { Status.Postponed, "NEEDS-ACTION" },
        { Status.Someday, "NEEDS-ACTION" },
        { Status.Canceled, "CANCELLED" },
        { Status.Reference, "IN-PROCESS" },
    };

    private static readonly Dictionary<string, Status> gtdStateMapper = new()
    {
        { "NEEDS-ACTION", Status.Waiting },
        { "IN-PROCESS", Status.Active },
        { "CANCELLED", Status.Canceled },
    };

    public static string MapStatus(this GTDTaskModel src, bool isCompleted)
    {
        if (isCompleted)
            return "COMPLETED";

        if (intermediateStateMapper.TryGetValue(src.Status, out var status))
            return status;

        return "IN-PROCESS";
    }

    public static Status MapStatus(this string? src)
    {
        if (src != null && gtdStateMapper.TryGetValue(src, out var status))
            return status;

        return Status.Active;
    }
}
