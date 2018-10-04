using System.Collections.Generic;
using Newtonsoft.Json;

namespace Manisero.Navvy.Reporting.Utils
{
    internal static class ObjectUtils
    {
        public static string ConcatString<TValue>(
            this IEnumerable<TValue> values)
            => string.Concat(values);

        public static string JoinWithSeparator<TValue>(
            this IEnumerable<TValue> values,
            string separator)
            => string.Join(separator, values);

        public static IEnumerable<TValue> ToEnumerable<TValue>(
            this TValue value)
        {
            yield return value;
        }

        public static string ToJson(
            this object value)
            => JsonConvert.SerializeObject(value);
    }
}
