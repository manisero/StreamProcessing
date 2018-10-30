using System.Collections.Generic;
using BankApp.Domain.WideKeys.Data;
using Dapper;
using DataProcessing.Utils;
using Npgsql;

namespace BankApp.DataAccess.Partitioned.Data
{
    public class LoanSnapshotRepository : WideKeys.Data.LoanSnapshotRepository
    {
        public LoanSnapshotRepository(
            string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>Returns ClientId -> Loans</summary>
        public IDictionary<int, ICollection<LoanSnapshot>> GetRange(
            short datasetId,
            int firstClientId,
            int lastClientId)
        {
            var sql = $@"
SELECT *
FROM ""{nameof(LoanSnapshot)}""
WHERE
  ""{nameof(LoanSnapshot.DatasetId)}"" = @DatasetId AND
  ""{nameof(LoanSnapshot.ClientId)}"" BETWEEN @FirstClientId AND @LastClientId";

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                return connection
                    .Query<LoanSnapshot>(
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
