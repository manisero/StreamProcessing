using System.Collections.Generic;
using BankApp.Domain.WideKeys;
using BankApp.Domain.WideKeys.Data;
using Dapper;
using DataProcessing.Utils;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp8.Common.DataAccess
{
    public class LoanSnapshotRepository
    {
        private readonly string _connectionString;

        public LoanSnapshotRepository(
            string connectionString)
        {
            _connectionString = connectionString;
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

            using (var connection = new NpgsqlConnection(_connectionString))
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

        public void CreateMany(
            IEnumerable<LoanSnapshot> items)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                PostgresCopyExecutor.Execute(
                    connection,
                    items,
                    LoanSnapshot.ColumnMapping);
            }
        }
    }
}
