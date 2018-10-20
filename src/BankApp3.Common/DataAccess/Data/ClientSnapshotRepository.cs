using System.Collections.Generic;
using BankApp.Domain.WideKeys.Data;
using Dapper;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp3.Common.DataAccess.Data
{
    public class ClientSnapshotRepository
    {
        private readonly string _connectionString;

        public ClientSnapshotRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public ICollection<ClientSnapshot> GetForDataset(
            short datasetId)
        {
            var sql = $@"
SELECT * FROM ""{nameof(ClientSnapshot)}""
WHERE ""{nameof(ClientSnapshot.DatasetId)}"" = @DatasetId";

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
                PostgresCopyExecutor.Execute(
                    connection,
                    items,
                    ClientSnapshot.ColumnMapping);
            }
        }
    }
}
