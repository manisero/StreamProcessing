using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Dapper;

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
            var parameters = new DynamicParameters();

            parameters.Add("BatchSize", batchSize);

            var whereClause = FormatWhereClause(previousBatchLastRowKey, filter, parameters);
            var orderClause = FormatOrderClause(previousBatchLastRowKey.Keys);

            var query = $@"
select *
from ""{tableName}""
where {whereClause}
order by {orderClause}
limit @BatchSize";

            return connection.Query<TRow>(query, parameters).AsList();
        }

        private static string FormatWhereClause(
            IDictionary<string, int> previousBatchLastRowKey,
            IDictionary<string, int> filter,
            DynamicParameters parametersToFill)
        {
            return "1 = 1";
        }

        private static string FormatOrderClause(
            IEnumerable<string> keyColumns)
            => string.Join(", ", keyColumns.Select(x => $"\"{x}\""));
    }
}
