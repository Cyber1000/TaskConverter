using Ical.Net;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Utils;

public static class MapStatusExtension
{
    private static readonly Dictionary<Status, string> intermediateStateMapper = new()
    {
        { Status.None, ToDoParticipationStatus.NeedsAction },
        { Status.NextAction, ToDoParticipationStatus.InProcess },
        { Status.Active, ToDoParticipationStatus.Accepted },
        { Status.Planning, ToDoParticipationStatus.InProcess },
        { Status.Delegated, ToDoParticipationStatus.Delegated },
        { Status.Waiting, ToDoParticipationStatus.NeedsAction },
        { Status.Hold, ToDoParticipationStatus.NeedsAction },
        { Status.Postponed, ToDoParticipationStatus.NeedsAction },
        { Status.Someday, ToDoParticipationStatus.Tentative },
        { Status.Canceled, ToDoParticipationStatus.Declined },
        { Status.Reference, ToDoParticipationStatus.InProcess },
    };

    private static readonly Dictionary<string, Status> gtdStateMapper = new()
    {
        { ToDoParticipationStatus.NeedsAction, Status.Waiting },
        { ToDoParticipationStatus.InProcess, Status.NextAction },
        { ToDoParticipationStatus.Accepted, Status.Active },
        { ToDoParticipationStatus.Declined, Status.Canceled },
        { ToDoParticipationStatus.Delegated, Status.Delegated },
        { ToDoParticipationStatus.Tentative, Status.Someday },
    };

    public static string MapStatus(this GTDTaskModel src, bool isCompleted)
    {
        if (isCompleted)
            return ToDoParticipationStatus.Completed;

        if (intermediateStateMapper.TryGetValue(src.Status, out var status))
            return status;

        return ToDoParticipationStatus.InProcess;
    }

    public static Status MapStatus(this string? src)
    {
        if (src != null && gtdStateMapper.TryGetValue(src, out var status))
            return status;

        return Status.Active;
    }
}
