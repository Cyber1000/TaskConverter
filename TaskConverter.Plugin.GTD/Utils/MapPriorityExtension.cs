using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Utils;

/// <summary>
/// GTD knows five priorities, RFC 5545 uses 0 for "undefined" and 1 (highest) to 9 (lowest).
/// Each GTD value gets its own anchor inside that range, so our own values survive a roundtrip
/// exactly, while any value a foreign client writes still maps onto something sensible.
/// </summary>
public static class MapPriorityExtension
{
    private static readonly Dictionary<Priority, int> intermediatePriorityMapper = new()
    {
        { Priority.None, 0 },
        { Priority.Top, 1 },
        { Priority.High, 3 },
        { Priority.Med, 5 },
        { Priority.Low, 7 },
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
