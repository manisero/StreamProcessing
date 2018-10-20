using System;
using System.Collections.Generic;
using BankApp.Domain.WideKeys;
using Dapper;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp3.Common.DataAccess
{
    public class ClientSnapshotRepository
    {
        private static readonly Dictionary<string, Action<NpgsqlBinaryImporter, ClientSnapshot>> ColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, ClientSnapshot>>
            {
                [nameof(ClientSnapshot.ClientId)] = (writer, x) => writer.Write(x.ClientId),
                [nameof(ClientSnapshot.DatasetId)] = (writer, x) => writer.Write(x.DatasetId)
            };

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
                    ColumnMapping);
            }
        }
    }
}
