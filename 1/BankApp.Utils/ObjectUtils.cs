using System.Collections.Generic;

namespace BankApp.Utils
{
    public static class ObjectUtils
    {
        public static string ConcatString<TValue>(
            this IEnumerable<TValue> values)
            => string.Concat(values);

        public static string JoinWithSeparator<TValue>(
            this IEnumerable<TValue> values,
            string separator)
            => string.Join(separator, values);
    }
}
