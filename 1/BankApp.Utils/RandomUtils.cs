using System;
using System.Collections.Generic;

namespace BankApp.Utils
{
    public static class RandomUtils
    {
        public static ICollection<int> NextUniqueCollection(
            this Random random,
            int m,
            int n)
        {
            var result = new HashSet<int>();
            
            for (var j = n - m + 1; j <= n; j++)
            {
                var t = random.Next(1, j);

                if (!result.Add(t))
                {
                    result.Add(j);
                }
            }

            return result;
        }
    }
}
