using System.Collections.Generic;
using Dapper;
using Manisero.StreamProcessing.Domain;
using Manisero.StreamProcessing.Utils;
using Manisero.StreamProcessing.Utils.DataAccess.BatchedReading;
using Npgsql;

namespace Manisero.StreamProcessing.Process.DataAccess
{
    public interface ILoanRepository
    {
        BatchedDataReader<Loan> GetBatchedReader(
            short datasetId,
            int batchSize = Loan.DefaultReadingBatchSize);

        /// <summary>Returns ClientId -> Loans</summary>
        IDictionary<int, ICollection<Loan>> GetRange(
            short datasetId,
            int firstClientId,
            int lastClientId);
    }

    public class LoanRepository : ILoanRepository
    {
        private readonly string _connectionString;

        public LoanRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public BatchedDataReader<Loan> GetBatchedReader(
            short datasetId,
            int batchSize = Loan.DefaultReadingBatchSize)
        {
            return new BatchedDataReader<Loan>(
                () => new NpgsqlConnection(_connectionString),
                nameof(Loan),
                new[] { nameof(Loan.DatasetId), nameof(Loan.ClientId), nameof(Loan.LoanId) },
                batchSize,
                new Dictionary<string, int> { [nameof(Client.DatasetId)] = datasetId });
        }
        
        public IDictionary<int, ICollection<Loan>> GetRange(
            short datasetId,
            int firstClientId,
            int lastClientId)
        {
            const string sql = @"
select *
from ""Loan""
where
  ""DatasetId"" = @DatasetId and
  ""ClientId"" between @FirstClientId and @LastClientId";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return connection
                    .Query<Loan>(
                        sql,
                        new
                        {
                            DatasetId = datasetId,
                            FirstClientId = firstClientId,
                            LastClientId = lastClientId
                        },
                        buffered: false)
                    .GroupAndDict(
                        x => x.ClientId,
                        x => x.ToICollection());
            }
        }
    }
}
