using System.Collections.Generic;
using BankApp8.Common.Domain;
using Dapper;
using Npgsql;
using StreamProcessing.Utils.DatabaseAccess.BatchedReading;

namespace BankApp8.Common.DataAccess
{
    public interface IClientRepository
    {
        int CountInDataset(
            short datasetId);

        BatchedDataReader<Client> GetBatchedReader(
            short datasetId,
            int batchSize = Client.DefaultReadingBatchSize);
    }

    public class ClientRepository : IClientRepository
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
            int batchSize = Client.DefaultReadingBatchSize)
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
