using System.Text.RegularExpressions;

namespace TaskConverter.Plugin.GTD.Conversion;

public static partial class IntermediateFormatPropertyNames
{
    public static string CategoryMetaData(string keyWordName) => $"X-DGT-CATEGORY-{GetSanitizedKeyWordName(keyWordName)}";
    public static string HideUntil => "X-DGT-HIDE-UNTIL";
    public static string DueFloat => "X-DGT-DUE-FLOAT";
    public static string Starred => "X-DGT-STARRED";
    public static string Color => "X-DGT-COLOR";
    public static string IsVisible => "X-DGT-ISVISIBLE";
    public static string Start => "X-DGT-START";

    // RFC 5545 DATE-TIME values have second precision, the GTD backup format has millisecond
    // precision. Only the fraction is carried here, so it cannot drift apart from the date.
    public static string CreatedMilliseconds => "X-DGT-CREATED-MS";
    public static string ModifiedMilliseconds => "X-DGT-MODIFIED-MS";
    public static string CompletedMilliseconds => "X-DGT-COMPLETED-MS";

    // VTODO has no equivalent of project, checklist and the other GTD task types
    public static string TaskType => "X-DGT-TASK-TYPE";

    // These three used to be derived on the way back - Hide from comparing due date and hide date,
    // DueDateModifier from Floating, DueTimeSet from the due date having a time. Real data
    // disagrees with all three, so the value is carried and the derivation is only a fallback.
    public static string Hide => "X-DGT-HIDE";
    public static string DueDateModifier => "X-DGT-DUE-DATE-MODIFIER";
    public static string DueTimeSet => "X-DGT-DUE-TIME-SET";

    private static string GetSanitizedKeyWordName(string keyWordName) => SanitizeRegex().Replace(keyWordName, "");

    [GeneratedRegex("[^a-zA-Z0-9_-]")]
    private static partial Regex SanitizeRegex();
}
