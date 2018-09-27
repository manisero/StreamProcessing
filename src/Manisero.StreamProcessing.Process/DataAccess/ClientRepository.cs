using System.Collections.Generic;
using Dapper;
using Manisero.StreamProcessing.Domain;
using Manisero.StreamProcessing.Utils.DataAccess.BatchedReading;
using Npgsql;

namespace Manisero.StreamProcessing.Process.DataAccess
{
    public class ClientRepository
    {
        private readonly string _connectionString;

        public ClientRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public int CountInDataset(
            short datasetId)
        {
            const string sql = @"
select count(*)
from ""Client""
where ""DatasetId"" = @DatasetId";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return connection.QuerySingle<int>(sql, new { DatasetId = datasetId });
            }
        }

        public BatchedDataReader<Client> GetBatchedReader(
            short datasetId,
            int batchSize = 100000)
        {
            return new BatchedDataReader<Client>(
                () => new NpgsqlConnection(_connectionString),
                nameof(Client),
                new[] { nameof(Client.DatasetId), nameof(Client.ClientId) },
                batchSize,
                new Dictionary<string, int> { [nameof(Client.DatasetId)] = datasetId });
        }
    }
}
