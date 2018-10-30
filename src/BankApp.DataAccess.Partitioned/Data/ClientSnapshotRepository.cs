using System.Collections.Generic;
using System.Linq;
using BankApp.Domain.WideKeys;
using BankApp.Domain.WideKeys.Data;
using DataProcessing.Utils.DatabaseAccess.BatchedReading;
using Npgsql;

namespace BankApp.DataAccess.Partitioned.Data
{
    public class ClientSnapshotRepository : WideKeys.Data.ClientSnapshotRepository
    {
        public ClientSnapshotRepository(
            string connectionString)
            : base(connectionString)
        {
        }

        public int CountInDataset(
            short datasetId)
        {
            using (var context = new EfContext(ConnectionString))
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
                () => new NpgsqlConnection(ConnectionString),
                nameof(ClientSnapshot),
                new[] { nameof(ClientSnapshot.DatasetId), nameof(ClientSnapshot.ClientId) },
                batchSize,
                new Dictionary<string, int> { [nameof(ClientSnapshot.DatasetId)] = datasetId });
        }
    }
}
