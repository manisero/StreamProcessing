using System.Collections.Generic;
using BankApp.Domain.WideKeys;
using BankApp.Domain.WideKeys.Data;
using Dapper;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp3.Common.DataAccess
{
    public class LoanSnapshotRepository
    {
        private readonly string _connectionString;

        public LoanSnapshotRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public ICollection<LoanSnapshot> GetForDataset(
            short datasetId)
        {
            var sql = $@"
SELECT * FROM ""{nameof(LoanSnapshot)}""
WHERE ""{nameof(LoanSnapshot.DatasetId)}"" = @DatasetId";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return connection
                    .Query<LoanSnapshot>(sql, new { DatasetId = datasetId })
                    .AsList();
            }
        }

        public void CreateMany(
            IEnumerable<LoanSnapshot> items)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                PostgresCopyExecutor.Execute(
                    connection,
                    items,
                    LoanSnapshot.ColumnMapping);
            }
        }
    }
}
