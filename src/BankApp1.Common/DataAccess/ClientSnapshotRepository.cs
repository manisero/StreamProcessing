using System;
using System.Collections.Generic;
using System.Linq;
using BankApp1.Common.Domain;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp1.Common.DataAccess
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
            int datasetId)
        {
            using (var context = new EfContext(_connectionString))
            {
                return context.Set<ClientSnapshot>().Where(x => x.DatasetId == datasetId).ToList();
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
