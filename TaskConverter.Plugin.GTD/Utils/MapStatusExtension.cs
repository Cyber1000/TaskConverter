using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Utils;

public static class MapStatusExtension
{
    public static string MapStatus(this GTDTaskModel src)
    {
        //TODO HH: better Map, RFC 5545 only knows NEEDS-ACTION, COMPLETED, IN-PROCESS and CANCELLED, so if src.Completed != null this should be set to completed
        return src.Status.ToString();
    }
}
