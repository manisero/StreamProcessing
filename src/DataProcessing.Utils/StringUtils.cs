using System.Collections.Generic;
using System.Linq;

namespace DataProcessing.Utils
{
    public static class StringUtils
    {
        public static string ToQuotedCommaSeparatedString(
            this IEnumerable<string> values)
            => values.Select(x => $@"""{x}""").JoinWithSeparator(", ");
    }
}
