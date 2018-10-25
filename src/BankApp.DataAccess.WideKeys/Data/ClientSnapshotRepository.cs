using System.Collections.Generic;
using BankApp.Domain.WideKeys.Data;
using Dapper;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp.DataAccess.WideKeys.Data
{
    public class ClientSnapshotRepository
    {
        protected readonly string ConnectionString;

        public ClientSnapshotRepository(
            string connectionString)
        {
            ConnectionString = connectionString;
        }

        public ICollection<ClientSnapshot> GetForDataset(
            short datasetId)
        {
            var sql = $@"
SELECT *
FROM ""{nameof(ClientSnapshot)}""
WHERE ""{nameof(ClientSnapshot.DatasetId)}"" = @DatasetId
ORDER BY ""{nameof(ClientSnapshot.DatasetId)}"", ""{nameof(ClientSnapshot.ClientId)}""";

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                return connection
                    .Query<ClientSnapshot>(sql, new { DatasetId = datasetId })
                    .AsList();
            }
        }

        public void CreateMany(
            IEnumerable<ClientSnapshot> items)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                PostgresCopyExecutor.ExecuteWrite(
                    connection,
                    items,
                    ClientSnapshot.ColumnMapping);
            }
        }
    }

    public class ClientSnapshotRepositoryWithSchema : ClientSnapshotRepository
    {
        private readonly bool _hasPk;
        private readonly bool _hasFk;

        public ClientSnapshotRepositoryWithSchema(
            string connectionString,
            bool hasPk = true,
            bool hasFk = true)
            : base(connectionString)
        {
            _hasPk = hasPk;
            _hasFk = hasFk;
        }

        public void DropConstraints()
        {
            if (_hasFk)
            {
                DatabaseManager.DropFk<ClientSnapshot, Dataset>(
                    ConnectionString,
                    nameof(ClientSnapshot.DatasetId));
            }

            if (_hasPk)
            {
                DatabaseManager.DropPk<ClientSnapshot>(
                    ConnectionString);
            }
        }

        public void RestoreConstraints()
        {
            if (_hasPk)
            {
                DatabaseManager.CreatePk<ClientSnapshot>(
                    ConnectionString,
                    nameof(ClientSnapshot.DatasetId), nameof(ClientSnapshot.ClientId));
            }

            if (_hasFk)
            {
                DatabaseManager.CreateFk<ClientSnapshot, Dataset>(
                    ConnectionString,
                    nameof(ClientSnapshot.DatasetId));
            }
        }
    }
}
