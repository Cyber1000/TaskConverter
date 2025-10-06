using System.Text;

namespace TaskConverter.Plugin.Base.Utils
{
    public static class HashUtil
    {
        public static int Fnv1aHashInt(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return 0;

            const uint fnvPrime = 16777619;
            const uint fnvOffsetBasis = 2166136261;

            uint hash = fnvOffsetBasis;
            foreach (byte b in Encoding.UTF8.GetBytes(input))
            {
                hash ^= b;
                hash *= fnvPrime;
            }

            return (int)(hash & 0x7FFFFFFF);
        }
    }
}
