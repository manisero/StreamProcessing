using Manisero.StreamProcessing.Domain;
using Manisero.StreamProcessing.Utils.DataAccess.BatchedReading;

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

        public BatchedDataReader<Client> GetBatchedReader(
            short datasetId,
            int batchSize = 100000)
        {
            return new BatchedDataReader<Client>(
                $"\"{nameof(Client)}\"",
                new[] { nameof(Client.DatasetId), nameof(Client.ClientId) },
                _connectionString,
                batchSize);
        }
    }
}
