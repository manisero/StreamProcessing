using System;
using System.Collections.Generic;
using BankApp.Domain;
using BankApp.Utils.DatabaseAccess;
using Npgsql;
using NpgsqlTypes;

namespace BankApp.DataAccess
{
    public class ClientSnapshotRepository
    {
        private static readonly Dictionary<string, Action<NpgsqlBinaryImporter, ClientSnapshot>> ColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, ClientSnapshot>>
            {
                [nameof(ClientSnapshot.ClientId)] = (writer, x) => writer.Write(x.ClientId, NpgsqlDbType.Integer),
                [nameof(ClientSnapshot.DatasetId)] = (writer, x) => writer.Write(x.DatasetId, NpgsqlDbType.Integer)
            };

        private readonly string _connectionString;

        public ClientSnapshotRepository(
            string connectionString)
        {
            _connectionString = connectionString;
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
