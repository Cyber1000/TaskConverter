namespace TaskConverter.Commons.ConversionHelper;

public static class BoolConverterExtensions
{
    public static bool ToBool(this string? input)
    {
        return bool.TryParse(input, out var boolConvert) && boolConvert;
    }

    //TODO HH: other direction?
}
