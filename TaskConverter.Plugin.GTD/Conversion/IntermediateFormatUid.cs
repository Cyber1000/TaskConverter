using TaskConverter.Plugin.Base.ConversionHelper;
using TaskConverter.Plugin.GTD.Model;

namespace TaskConverter.Plugin.GTD.Conversion;

/// <summary>
/// GTD ids are unique per entity type only, so a task and a notebook can share one. The UID has to
/// separate them, because it identifies a component across the whole calendar and IcalWriter derives
/// its file name from it. The prefix keeps the UID readable and file system safe.
/// </summary>
public static class IntermediateFormatUid
{
    public const string TaskPrefix = "task";
    public const string NotebookPrefix = "notebook";

    private static readonly Dictionary<Type, string> prefixByModelType = new()
    {
        { typeof(GTDTaskModel), TaskPrefix },
        { typeof(GTDNotebookModel), NotebookPrefix },
        { typeof(GTDFolderModel), "folder" },
        { typeof(GTDContextModel), "context" },
        { typeof(GTDTagModel), "tag" },
        { typeof(GTDTaskNoteModel), "tasknote" },
    };

    public static string ToUid(GTDBaseModel model) => ToUid(prefixByModelType.GetValueOrDefault(model.GetType()), model.Id);

    public static string ToUid(string? prefix, int id) => string.IsNullOrEmpty(prefix) ? id.ToString() : $"{prefix}-{id}";

    /// <summary>
    /// Accepts our own prefixed uids as well as anything a foreign client may have written, which
    /// falls back to the hash so that it at least stays stable.
    /// </summary>
    public static int ToId(string? uid)
    {
        if (string.IsNullOrEmpty(uid))
            return 0;

        var separatorIndex = uid.IndexOf('-');
        if (separatorIndex > 0 && prefixByModelType.ContainsValue(uid[..separatorIndex]) && int.TryParse(uid[(separatorIndex + 1)..], out var id))
            return id;

        return uid.ToIntWithHashFallback();
    }
}
