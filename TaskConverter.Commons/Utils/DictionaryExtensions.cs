namespace TaskConverter.Commons.Utils;

public static class DictionaryExtensions
{
    /// <summary>
    /// Returns the values from the dictionary for the given set of keys.
    /// Null or missing keys will be skipped.
    /// </summary>
    public static IEnumerable<TValue> GetExistingValues<TKey, TValue>(this IEnumerable<TKey> keys, IDictionary<TKey, TValue>? dictionary)
        where TKey : notnull
    {
        if (keys is null || dictionary is null)
            return Enumerable.Empty<TValue>();

        return keys.Where(dictionary.ContainsKey).Select(k => dictionary[k]);
    }
}
