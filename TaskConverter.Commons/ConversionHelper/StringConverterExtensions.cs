using TaskConverter.Commons.Utils;

namespace TaskConverter.Commons.ConversionHelper;

public static class StringConverterExtensions
{
    public static int ToIntWithHashFallback(this string? input)
    {
        return int.TryParse(input, out var id) ? id : HashUtil.Fnv1aHashInt(input);
    }

    //TODO HH: other direction?
}
