using System.Collections.Generic;
using System.Data.Common;

namespace Manisero.StreamProcessing.Utils.DataAccess.BatchedReading
{
    public static class BatchReader
    {
        public static ICollection<TRow> Read<TRow>(
            DbConnection connection,
            string tableName,
            IDictionary<string, int> previousBatchLastRowKey,
            int batchSize,
            IDictionary<string, int> filter = null)
        {
            var whereClause = FormatWhereClause();
            var orderClause = FormatOrderClause();

            var query =
$@"SELECT TOP({batchSize}) *
FROM {tableName}
WHERE {whereClause}
ORDER BY {orderClause}";

            return new TRow[0];
        }

        private static string FormatWhereClause()
        {
            return null;
        }

        private static string FormatOrderClause()
        {
            return null;
        }
    }
}
