namespace TaskConverter.Commons.ConversionHelper;

public static class BoolConverterExtensions
{
    public static bool ToBool(this string? input)
    {
        return bool.TryParse(input, out var boolConvert) && boolConvert;
    }

    public static string ToStringRepresentation(this bool input)
    {
        return input.ToString().ToLowerInvariant();
    }
}
