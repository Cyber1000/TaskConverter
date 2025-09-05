using System.Drawing;

namespace TaskConverter.Commons.ConversionHelper;

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
            _ => Color.FromArgb(value)
        };
    }
}
