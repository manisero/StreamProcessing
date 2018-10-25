using System.Collections.Generic;
using BankApp.Domain.WideKeys.Data;
using Dapper;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp.DataAccess.WideKeys.Data
{
    public class DepositSnapshotRepository
    {
        protected readonly string ConnectionString;

        public DepositSnapshotRepository(
            string connectionString)
        {
            ConnectionString = connectionString;
        }

        public ICollection<DepositSnapshot> GetForDataset(
            short datasetId)
        {
            var sql = $@"
SELECT *
FROM ""{nameof(DepositSnapshot)}""
WHERE ""{nameof(DepositSnapshot.DatasetId)}"" = @DatasetId
ORDER BY ""{nameof(DepositSnapshot.DatasetId)}"", ""{nameof(DepositSnapshot.ClientId)}"", ""{nameof(DepositSnapshot.DepositId)}""";

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                return connection
                    .Query<DepositSnapshot>(sql, new { DatasetId = datasetId })
                    .AsList();
            }
        }

        public void CreateMany(
            IEnumerable<DepositSnapshot> items)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                PostgresCopyExecutor.ExecuteWrite(
                    connection,
                    items,
                    DepositSnapshot.ColumnMapping);
            }
        }
    }

    public class DepositSnapshotRepositoryWithSchema : DepositSnapshotRepository
    {
        private readonly bool _hasPk;
        private readonly bool _hasFk;

        public DepositSnapshotRepositoryWithSchema(
            string connectionString,
            bool hasPk = true,
            bool hasFk = false)
            : base(connectionString)
        {
            _hasPk = hasPk;
            _hasFk = hasFk;
        }

        public void DropConstraints()
        {
            if (_hasFk)
            {
                DatabaseManager.DropFk<DepositSnapshot, ClientSnapshot>(
                    ConnectionString,
                    nameof(DepositSnapshot.DatasetId), nameof(DepositSnapshot.ClientId));
            }

            if (_hasPk)
            {
                DatabaseManager.DropPk<DepositSnapshot>(
                    ConnectionString);
            }
        }

        public void RestoreConstraints()
        {
            if (_hasPk)
            {
                DatabaseManager.CreatePk<DepositSnapshot>(
                    ConnectionString,
                    nameof(DepositSnapshot.DatasetId), nameof(DepositSnapshot.ClientId), nameof(DepositSnapshot.DepositId));
            }

            if (_hasFk)
            {
                DatabaseManager.CreateFk<DepositSnapshot, ClientSnapshot>(
                    ConnectionString,
                    nameof(DepositSnapshot.DatasetId), nameof(DepositSnapshot.ClientId));
            }
        }
    }
}
