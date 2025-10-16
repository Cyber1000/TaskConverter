using System.Drawing;

namespace TaskConverter.Plugin.Base.ConversionHelper;

public static class ColorConverterExtensions
{
    public static int ToArgbWithFallback(this Color? input)
    {
        if (input == null)
        {
            return -1;
        }

        if (input.Value.IsEmpty)
        {
            return -2;
        }

        return input.Value.ToArgb();
    }

    public static Color? FromArgbWithFallback(this int value)
    {
        return value switch
        {
            -1 => null,
            -2 => Color.Empty,
            _ => Color.FromArgb(value),
        };
    }

    public static string ToStringRepresentation(this Color? value) => ToArgbWithFallback(value).ToString();

    public static Color? ColorFromStringRepresentation(this string? value) => FromArgbWithFallback(ColorIntFromStringRepresentation(value));

    public static int ColorIntFromStringRepresentation(this string? value)
    {
        if (value == null || !int.TryParse(value, out var colorInt))
            return -1;

        return colorInt;
    }
}
