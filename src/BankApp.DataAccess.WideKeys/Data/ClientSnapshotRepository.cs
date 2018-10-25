using System.Collections.Generic;
using BankApp.Domain.WideKeys.Data;
using Dapper;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp.DataAccess.WideKeys.Data
{
    public class ClientSnapshotRepository
    {
        private readonly string _connectionString;
        private readonly bool _hasFk;
        private readonly bool _hasPk;

        public ClientSnapshotRepository(
            string connectionString,
            bool hasFk = true,
            bool hasPk = true)
        {
            _connectionString = connectionString;
            _hasFk = hasFk;
            _hasPk = hasPk;
        }

        public ICollection<ClientSnapshot> GetForDataset(
            short datasetId)
        {
            var sql = $@"
SELECT *
FROM ""{nameof(ClientSnapshot)}""
WHERE ""{nameof(ClientSnapshot.DatasetId)}"" = @DatasetId
ORDER BY ""{nameof(ClientSnapshot.DatasetId)}"", ""{nameof(ClientSnapshot.ClientId)}""";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return connection
                    .Query<ClientSnapshot>(sql, new { DatasetId = datasetId })
                    .AsList();
            }
        }

        public void CreateMany(
            IEnumerable<ClientSnapshot> items)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                PostgresCopyExecutor.ExecuteWrite(
                    connection,
                    items,
                    ClientSnapshot.ColumnMapping);
            }
        }

        public void DropConstraints()
        {
            if (_hasFk)
            {
                DatabaseManager.DropFk<ClientSnapshot, Dataset>(
                    _connectionString,
                    nameof(ClientSnapshot.DatasetId));
            }

            if (_hasPk)
            {
                DatabaseManager.DropPk<ClientSnapshot>(
                    _connectionString);
            }
        }

        public void RestoreConstraints()
        {
            if (_hasPk)
            {
                DatabaseManager.CreatePk<ClientSnapshot>(
                    _connectionString,
                    nameof(ClientSnapshot.DatasetId), nameof(ClientSnapshot.ClientId));
            }

            if (_hasFk)
            {
                DatabaseManager.CreateFk<ClientSnapshot, Dataset>(
                    _connectionString,
                    nameof(ClientSnapshot.DatasetId));
            }
        }
    }
}
