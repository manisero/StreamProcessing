using System.Collections.Generic;
using BankApp.Domain.WideKeys;
using Dapper;
using DataProcessing.Utils.DatabaseAccess;
using DataProcessing.Utils.DatabaseAccess.BatchedReading;
using Npgsql;

namespace BankApp8.Common.DataAccess
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
            var sql = $@"
select count(*)
from ""{nameof(ClientSnapshot)}""
where ""{nameof(ClientSnapshot.DatasetId)}"" = @DatasetId";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return connection.QuerySingle<int>(sql, new { DatasetId = datasetId });
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
                PostgresCopyExecutor.Execute(
                    connection,
                    items,
                    ClientSnapshot.ColumnMapping);
            }
        }
    }
}
