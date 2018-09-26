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
            return new TRow[0];
        }
    }
}
