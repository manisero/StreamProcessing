using System.Collections.Generic;
using System.Linq;
using BankApp.Domain.WideKeys;
using BankApp.Domain.WideKeys.Data;
using DataProcessing.Utils.DatabaseAccess;
using DataProcessing.Utils.DatabaseAccess.BatchedReading;
using Npgsql;

namespace BankApp.DataAccess.Partitioned.Data
{
    public class ClientSnapshotRepository
    {
        private readonly string _connectionString;

        public ClientSnapshotRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public int CountInDataset(
            short datasetId)
        {
            using (var context = new EfContext(_connectionString))
            {
                return context
                    .Set<ClientSnapshot>()
                    .Count(x => x.DatasetId == datasetId);
            }
        }

        public BatchedDataReader<ClientSnapshot> GetBatchedReader(
            short datasetId,
            int batchSize = ClientSnapshot.DefaultReadingBatchSize)
        {
            return new BatchedDataReader<ClientSnapshot>(
                () => new NpgsqlConnection(_connectionString),
                nameof(ClientSnapshot),
                new[] { nameof(ClientSnapshot.DatasetId), nameof(ClientSnapshot.ClientId) },
                batchSize,
                new Dictionary<string, int> { [nameof(ClientSnapshot.DatasetId)] = datasetId });
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
    }
}
