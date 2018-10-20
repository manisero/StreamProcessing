using System.Collections.Generic;
using BankApp.Domain.WideKeys;
using BankApp.Domain.WideKeys.Data;
using Dapper;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp3.Common.DataAccess
{
    public class DepositSnapshotRepository
    {
        private readonly string _connectionString;

        public DepositSnapshotRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public ICollection<DepositSnapshot> GetForDataset(
            short datasetId)
        {
            var sql = $@"
SELECT * FROM ""{nameof(DepositSnapshot)}""
WHERE ""{nameof(DepositSnapshot.DatasetId)}"" = @DatasetId";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return connection
                    .Query<DepositSnapshot>(sql, new { DatasetId = datasetId })
                    .AsList();
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
