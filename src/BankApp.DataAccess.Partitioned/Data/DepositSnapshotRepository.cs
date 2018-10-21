using System.Collections.Generic;
using BankApp.Domain.WideKeys.Data;
using Dapper;
using DataProcessing.Utils;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp.DataAccess.Partitioned.Data
{
    public class DepositSnapshotRepository
    {
        private readonly string _connectionString;

        public DepositSnapshotRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>Returns ClientId -> Deposits</summary>
        public IDictionary<int, ICollection<DepositSnapshot>> GetRange(
            short datasetId,
            int firstClientId,
            int lastClientId)
        {
            var sql = $@"
SELECT *
FROM ""{nameof(DepositSnapshot)}""
WHERE
  ""{nameof(DepositSnapshot.DatasetId)}"" = @DatasetId AND
  ""{nameof(DepositSnapshot.ClientId)}"" BETWEEN @FirstClientId AND @LastClientId";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return connection
                    .Query<DepositSnapshot>(
                        sql,
                        new
                        {
                            DatasetId = datasetId,
                            FirstClientId = firstClientId,
                            LastClientId = lastClientId
                        },
                        buffered: false)
                    .GroupAndDict(
                        x => x.ClientId,
                        x => x.ToICollection());
            }
        }

        public void CreateMany(
            IEnumerable<DepositSnapshot> items)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                PostgresCopyExecutor.Execute(
                    connection,
                    items,
                    DepositSnapshot.ColumnMapping);
            }
        }
    }
}
