using System.Collections.Generic;
using BankApp.Domain.WideKeys.Data;
using Dapper;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp.DataAccess.WideKeys.Data
{
    public class DepositSnapshotRepository
    {
        private readonly string _connectionString;

        public DepositSnapshotRepository(
            string connectionString,
            bool hasFk = true,
            bool hasPk = true)
        {
            _connectionString = connectionString;
        }

        public ICollection<DepositSnapshot> GetForDataset(
            short datasetId)
        {
            var sql = $@"
SELECT *
FROM ""{nameof(DepositSnapshot)}""
WHERE ""{nameof(DepositSnapshot.DatasetId)}"" = @DatasetId
ORDER BY ""{nameof(DepositSnapshot.DatasetId)}"", ""{nameof(DepositSnapshot.ClientId)}"", ""{nameof(DepositSnapshot.DepositId)}""";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return connection
                    .Query<DepositSnapshot>(sql, new { DatasetId = datasetId })
                    .AsList();
            }
        }

        public void CreateMany(
            IEnumerable<DepositSnapshot> items)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                PostgresCopyExecutor.ExecuteWrite(
                    connection,
                    items,
                    DepositSnapshot.ColumnMapping);
            }
        }

        public void DropConstraints()
        {
            var sql = $@"
ALTER TABLE ""{nameof(DepositSnapshot)}""
DROP CONSTRAINT ""FK_{nameof(DepositSnapshot)}_{nameof(ClientSnapshot)}_{nameof(DepositSnapshot.DatasetId)}_{nameof(DepositSnapshot.ClientId)}"";

ALTER TABLE ""{nameof(DepositSnapshot)}""
DROP CONSTRAINT ""PK_{nameof(DepositSnapshot)}"";";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Execute(sql);
            }
        }

        public void RestoreConstraints()
        {
            var sql = $@"
ALTER TABLE ""{nameof(DepositSnapshot)}""
ADD CONSTRAINT ""PK_{nameof(DepositSnapshot)}""
PRIMARY KEY (""{nameof(DepositSnapshot.DatasetId)}"", ""{nameof(DepositSnapshot.ClientId)}"", ""{nameof(DepositSnapshot.DepositId)}"");

ALTER TABLE ""{nameof(DepositSnapshot)}""
ADD CONSTRAINT ""FK_{nameof(DepositSnapshot)}_{nameof(ClientSnapshot)}_{nameof(DepositSnapshot.DatasetId)}_{nameof(DepositSnapshot.ClientId)}""
FOREIGN KEY (""{nameof(DepositSnapshot.ClientId)}"", ""{nameof(DepositSnapshot.DatasetId)}"")
REFERENCES ""{nameof(ClientSnapshot)}"" (""{nameof(DepositSnapshot.ClientId)}"", ""{nameof(DepositSnapshot.DatasetId)}"");";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Execute(sql);
            }
        }
    }
}
