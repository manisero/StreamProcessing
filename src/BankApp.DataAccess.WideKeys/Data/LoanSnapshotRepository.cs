using System.Collections.Generic;
using BankApp.Domain.WideKeys.Data;
using Dapper;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp.DataAccess.WideKeys.Data
{
    public class LoanSnapshotRepository
    {
        private readonly string _connectionString;

        public LoanSnapshotRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public ICollection<LoanSnapshot> GetForDataset(
            short datasetId)
        {
            var sql = $@"
SELECT * FROM ""{nameof(LoanSnapshot)}""
WHERE ""{nameof(LoanSnapshot.DatasetId)}"" = @DatasetId
ORDER BY ""{nameof(LoanSnapshot.DatasetId)}"", ""{nameof(LoanSnapshot.ClientId)}"", ""{nameof(LoanSnapshot.LoanId)}""";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return connection
                    .Query<LoanSnapshot>(sql, new { DatasetId = datasetId })
                    .AsList();
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

        public void DropConstraints()
        {
            var sql = $@"
ALTER TABLE ""{nameof(LoanSnapshot)}""
DROP CONSTRAINT ""FK_{nameof(LoanSnapshot)}_{nameof(ClientSnapshot)}_{nameof(LoanSnapshot.DatasetId)}_{nameof(LoanSnapshot.ClientId)}"";

ALTER TABLE ""{nameof(LoanSnapshot)}""
DROP CONSTRAINT ""PK_{nameof(LoanSnapshot)}"";";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Execute(sql);
            }
        }

        public void RestoreConstraints()
        {
            var sql = $@"
ALTER TABLE ""{nameof(LoanSnapshot)}""
ADD CONSTRAINT ""PK_{nameof(LoanSnapshot)}""
PRIMARY KEY (""{nameof(LoanSnapshot.DatasetId)}"", ""{nameof(LoanSnapshot.ClientId)}"", ""{nameof(LoanSnapshot.LoanId)}"");

ALTER TABLE ""{nameof(LoanSnapshot)}""
ADD CONSTRAINT ""FK_{nameof(LoanSnapshot)}_{nameof(ClientSnapshot)}_{nameof(LoanSnapshot.DatasetId)}_{nameof(LoanSnapshot.ClientId)}""
FOREIGN KEY (""{nameof(LoanSnapshot.ClientId)}"", ""{nameof(LoanSnapshot.DatasetId)}"")
REFERENCES ""{nameof(ClientSnapshot)}"" (""{nameof(LoanSnapshot.ClientId)}"", ""{nameof(LoanSnapshot.DatasetId)}"");";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Execute(sql);
            }
        }
    }
}
