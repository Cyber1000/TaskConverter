namespace TaskConverter.Plugin.Base.Utils;

public static class StringExtensions
{
    private static readonly string[] newLineChars = ["\r\n", "\r", "\n"];

    public static string GetString(this string[] stringArray)
    {
        return string.Join("\n", stringArray);
    }

    public static string[] GetStringArray(this string currentString)
    {
        return currentString.Split(newLineChars, StringSplitOptions.None);
    }

    public static string RemovePrefix(this string s, string prefix)
    {
        if (!string.IsNullOrEmpty(prefix) && s.StartsWith(prefix))
            return s[prefix.Length..];
        return s;
    }

    public static string AddPrefix(this string s, string prefix)
    {
        if (!string.IsNullOrEmpty(prefix))
            return $"{prefix}{s}";
        return s;
    }

    public static bool StartsWithPrefix(this string s, string? prefix)
    {
        return !string.IsNullOrEmpty(prefix) && s.StartsWith(prefix);
    }
}
