using System;
using System.Collections.Generic;
using System.Linq;

namespace DataProcessing.Utils
{
    public static class EnumerableUtils
    {
        public static ICollection<TItem> ToICollection<TItem>(
            this IEnumerable<TItem> items)
            => items.ToArray();

        public static IDictionary<TKey, TValue> GroupAndDict<TItem, TKey, TValue>(
            this IEnumerable<TItem> items,
            Func<TItem, TKey> groupKeySelector,
            Func<IGrouping<TKey, TItem>, TValue> valueSelector)
        {
            return items
                .GroupBy(groupKeySelector)
                .ToDictionary(group => group.Key, valueSelector);
        }

        public static IEnumerable<TItem> Concat<TItem>(
            params IEnumerable<TItem>[] enumerables)
        {
            foreach (var enumerable in enumerables)
            {
                foreach (var item in enumerable)
                {
                    yield return item;
                }
            }
        }
    }
}
