using System;
using System.Collections.Generic;

namespace DataProcessing.Utils
{
    public static class RandomUtils
    {
        /// <summary>Generates collection of unique random numbers from 1 to specified max value.</summary>
        public static ICollection<int> NextUniqueCollection(
            this Random random,
            int count,
            int maxValueInclusive)
        {
            // Taken from https://stackoverflow.com/a/2394292/10401390

            if (maxValueInclusive < count)
            {
                throw new ArgumentException($"{nameof(maxValueInclusive)} cannot be lower than {nameof(count)}.", nameof(maxValueInclusive));
            }

            var m = count;
            var n = maxValueInclusive;
            var s = new HashSet<int>();

            for (var j = n - m + 1; j <= n; j++)
            {
                var t = random.Next(1, j);

                if (!s.Add(t))
                {
                    s.Add(j);
                }
            }

            return s;
        }
    }
}
