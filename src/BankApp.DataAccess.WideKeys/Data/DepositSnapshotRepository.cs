using System.Collections.Generic;
using BankApp.Domain.WideKeys.Data;
using Dapper;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp.DataAccess.WideKeys.Data
{
    public class DepositSnapshotRepository
    {
        private readonly string _connectionString;
        private readonly bool _hasFk;
        private readonly bool _hasPk;

        public DepositSnapshotRepository(
            string connectionString,
            bool hasFk = true,
            bool hasPk = true)
        {
            _connectionString = connectionString;
            _hasFk = hasFk;
            _hasPk = hasPk;
        }

        public ICollection<DepositSnapshot> GetForDataset(
            short datasetId)
        {
            var sql = $@"
SELECT *
FROM ""{nameof(DepositSnapshot)}""
WHERE ""{nameof(DepositSnapshot.DatasetId)}"" = @DatasetId
ORDER BY ""{nameof(DepositSnapshot.DatasetId)}"", ""{nameof(DepositSnapshot.ClientId)}"", ""{nameof(DepositSnapshot.DepositId)}""";

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
                PostgresCopyExecutor.ExecuteWrite(
                    connection,
                    items,
                    DepositSnapshot.ColumnMapping);
            }
        }

        public void DropConstraints()
        {
            if (_hasFk)
            {
                DatabaseManager.DropFk<DepositSnapshot, ClientSnapshot>(
                    _connectionString,
                    nameof(DepositSnapshot.DatasetId), nameof(DepositSnapshot.ClientId));
            }

            if (_hasPk)
            {
                DatabaseManager.DropPk<DepositSnapshot>(
                    _connectionString);
            }
        }

        public void RestoreConstraints()
        {
            if (_hasPk)
            {
                DatabaseManager.CreatePk<DepositSnapshot>(
                    _connectionString,
                    nameof(DepositSnapshot.DatasetId), nameof(DepositSnapshot.ClientId), nameof(DepositSnapshot.DepositId));
            }

            if (_hasFk)
            {
                DatabaseManager.CreateFk<DepositSnapshot, ClientSnapshot>(
                    _connectionString,
                    nameof(DepositSnapshot.DatasetId), nameof(DepositSnapshot.ClientId));
            }
        }
    }
}
