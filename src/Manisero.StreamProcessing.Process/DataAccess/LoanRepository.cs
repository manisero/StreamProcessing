using System.Collections.Generic;
using Manisero.StreamProcessing.Domain;
using Manisero.StreamProcessing.Utils.DataAccess.BatchedReading;
using Npgsql;

namespace Manisero.StreamProcessing.Process.DataAccess
{
    public class LoanRepository
    {
        private readonly string _connectionString;

        public LoanRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public BatchedDataReader<Loan> GetBatchedReader(
            short datasetId,
            int batchSize = 100000)
        {
            return new BatchedDataReader<Loan>(
                () => new NpgsqlConnection(_connectionString),
                nameof(Loan),
                new[] { nameof(Loan.DatasetId), nameof(Loan.ClientId), nameof(Loan.LoanId) },
                batchSize,
                new Dictionary<string, int> { [nameof(Client.DatasetId)] = datasetId });
        }
    }
}
