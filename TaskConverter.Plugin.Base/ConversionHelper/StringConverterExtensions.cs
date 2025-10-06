using TaskConverter.Plugin.Base.Utils;

namespace TaskConverter.Plugin.Base.ConversionHelper;

public static class StringConverterExtensions
{
    public static int ToIntWithHashFallback(this string? input)
    {
        return int.TryParse(input, out var id) ? id : HashUtil.Fnv1aHashInt(input);
    }
}
