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

    private static string GetSanitizedKeyWordName(string keyWordName) => SanitizeRegex().Replace(keyWordName, "");

    [GeneratedRegex("[^a-zA-Z0-9_-]")]
    private static partial Regex SanitizeRegex();
}
