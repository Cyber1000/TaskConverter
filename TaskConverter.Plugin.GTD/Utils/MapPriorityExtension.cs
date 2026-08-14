using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Utils;

/// <summary>
/// GTD knows five priorities, RFC 5545 uses 0 for "undefined" and 1 (highest) to 9 (lowest).
/// <para>
/// Low maps to 0, not to 7: it is the default of the GTD enum and therefore means "never touched"
/// rather than "low" - 5144 of 5998 tasks in a real backup carry it, and every one of them would
/// otherwise show up in the target app as an explicitly low priority task. That makes Low and None
/// indistinguishable here, which is why <see cref="IntermediateFormatPropertyNames.Priority"/>
/// carries the exact value alongside.
/// </para>
/// </summary>
public static class MapPriorityExtension
{
    private static readonly Dictionary<Priority, int> intermediatePriorityMapper = new()
    {
        { Priority.None, 0 },
        { Priority.Top, 1 },
        { Priority.High, 3 },
        { Priority.Med, 5 },
        { Priority.Low, 0 },
    };

    public static int MapPriority(this Priority priority) => intermediatePriorityMapper.TryGetValue(priority, out var mapped) ? mapped : 0;

    public static Priority MapPriority(this int icalPriority) =>
        icalPriority switch
        {
            <= 0 => Priority.None,
            <= 2 => Priority.Top,
            <= 4 => Priority.High,
            5 => Priority.Med,
            _ => Priority.Low,
        };
}
