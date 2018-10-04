using System;

namespace Manisero.Navvy.Reporting.Utils
{
    public static class DateTimeUtils
    {
        public static bool IsBetween(
            this DateTime value,
            DateTime from,
            DateTime to)
            => value >= from && value <= to;
    }
}
