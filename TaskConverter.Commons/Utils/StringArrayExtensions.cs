namespace TaskConverter.Commons.Utils;

public static class StringArrayExtensions
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
}
