using System.Collections.Generic;

namespace DataProcessing.Utils
{
    public static class DictionaryUtils
    {
        public static TValue GetValueOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> dict,
            TKey key)
            => dict.TryGetValue(key, out var value)
                ? value
                : default(TValue);
    }
}
