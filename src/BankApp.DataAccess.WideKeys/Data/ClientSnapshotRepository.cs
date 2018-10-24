using System.Collections.Generic;
using BankApp.Domain.WideKeys.Data;
using Dapper;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp.DataAccess.WideKeys.Data
{
    public class ClientSnapshotRepository
    {
        private readonly string _connectionString;

        public ClientSnapshotRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public ICollection<ClientSnapshot> GetForDataset(
            short datasetId)
        {
            var sql = $@"
SELECT *
FROM ""{nameof(ClientSnapshot)}""
WHERE ""{nameof(ClientSnapshot.DatasetId)}"" = @DatasetId
ORDER BY ""{nameof(ClientSnapshot.DatasetId)}"", ""{nameof(ClientSnapshot.ClientId)}""";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return connection
                    .Query<ClientSnapshot>(sql, new { DatasetId = datasetId })
                    .AsList();
            }
        }

        public void CreateMany(
            IEnumerable<ClientSnapshot> items)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                PostgresCopyExecutor.ExecuteWrite(
                    connection,
                    items,
                    ClientSnapshot.ColumnMapping);
            }
        }

        public void DropConstraints()
        {
            var sql = $@"
ALTER TABLE ""{nameof(ClientSnapshot)}""
DROP CONSTRAINT ""FK_{nameof(ClientSnapshot)}_{nameof(Dataset)}_{nameof(ClientSnapshot.DatasetId)}"";

ALTER TABLE ""{nameof(ClientSnapshot)}""
DROP CONSTRAINT ""PK_{nameof(ClientSnapshot)}"";";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Execute(sql);
            }
        }

        public void RestoreConstraints()
        {
            var sql = $@"
ALTER TABLE ""{nameof(ClientSnapshot)}""
ADD CONSTRAINT ""PK_{nameof(ClientSnapshot)}""
PRIMARY KEY (""{nameof(ClientSnapshot.DatasetId)}"", ""{nameof(ClientSnapshot.ClientId)}"");

ALTER TABLE ""{nameof(ClientSnapshot)}""
ADD CONSTRAINT ""FK_{nameof(ClientSnapshot)}_{nameof(Dataset)}_{nameof(ClientSnapshot.DatasetId)}""
FOREIGN KEY (""{nameof(ClientSnapshot.DatasetId)}"")
REFERENCES ""{nameof(Dataset)}"" (""{nameof(ClientSnapshot.DatasetId)}"");";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Execute(sql);
            }
        }
    }
}
