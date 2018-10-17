using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Dapper;

namespace DataProcessing.Utils.DatabaseAccess.BatchedReading
{
    public static class BatchReader
    {
        private static readonly IDictionary<string, int> EmptyFilter = new Dictionary<string, int>();

        public static ICollection<TRow> Read<TRow>(
            DbConnection connection,
            string tableName,
            IDictionary<string, int> previousBatchLastRowKey,
            int batchSize,
            IDictionary<string, int> filter = null)
        {
            var parameters = new DynamicParameters();

            parameters.Add("BatchSize", batchSize);

            var whereClause = FormatWhereClause(previousBatchLastRowKey, filter ?? EmptyFilter, parameters);
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
            if (filter.Any())
            {
                var filterWhere = BuildFilterWhere(filter, parametersToFill);
                var keyWhere = BuildKeyWhere(previousBatchLastRowKey, filter, parametersToFill);

                return $"{filterWhere}({keyWhere})";
            }
            else
            {
                return BuildKeyWhere(previousBatchLastRowKey, filter, parametersToFill);
            }
        }

        private static string BuildFilterWhere(
            IDictionary<string, int> filter,
            DynamicParameters parametersToFill)
        {
            return filter
                .Select((filterEntry, i) =>
                {
                    var paramName = $"Filter_{i}";
                    parametersToFill.Add(paramName, filterEntry.Value);
                    return $"\"{filterEntry.Key}\" = @{paramName} and ";
                })
                .ConcatString();
        }

        private static string BuildKeyWhere(
            IDictionary<string, int> previousBatchLastRowKey,
            IDictionary<string, int> filter,
            DynamicParameters parametersToFill)
        {
            var keysToFilter = new List<KeyValuePair<string, string>>(previousBatchLastRowKey.Count);

            return previousBatchLastRowKey
                .Where(x => !filter.ContainsKey(x.Key))
                .Select((keyEntry, i) =>
                {
                    var paramName = $"Key_{i}";
                    keysToFilter.Add(new KeyValuePair<string, string>(keyEntry.Key, paramName));
                    parametersToFill.Add(paramName, keyEntry.Value);

                    return keysToFilter
                        .Take(i)
                        .Select(x => $"\"{x.Key}\" = @{x.Value}")
                        .Append($"\"{keyEntry.Key}\" > @{paramName}")
                        .JoinWithSeparator(" and ");
                })
                .Select(x => $"({x})")
                .JoinWithSeparator(" or ");
        }

        private static string FormatOrderClause(
            IEnumerable<string> keyColumns)
            => string.Join(", ", keyColumns.Select(x => $"\"{x}\""));
    }
}
