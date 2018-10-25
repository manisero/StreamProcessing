using System.Collections.Generic;
using System.Linq;
using BankApp.Domain.WideKeys.Data;
using Dapper;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp.DataAccess.WideKeys.Data
{
    public class LoanSnapshotRepository
    {
        private readonly string _connectionString;
        private readonly bool _readUsingDapper;

        public LoanSnapshotRepository(
            string connectionString,
            bool readUsingDapper = false,
            bool hasFk = true,
            bool hasPk = true)
        {
            _connectionString = connectionString;
            _readUsingDapper = readUsingDapper;
        }

        public ICollection<LoanSnapshot> GetAll()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return PostgresCopyExecutor.ExecuteRead(
                    connection,
                    LoanSnapshot.RowReader);
            }
        }

        public ICollection<LoanSnapshot> GetForDataset(
            short datasetId)
        {
            return _readUsingDapper
                ? GetForDataset_Dapper(datasetId)
                : GetForDataset_Ef(datasetId);
        }

        private ICollection<LoanSnapshot> GetForDataset_Dapper(
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

        private ICollection<LoanSnapshot> GetForDataset_Ef(
            short datasetId)
        {
            using (var context = new EfContext(_connectionString))
            {
                return context
                    .Set<LoanSnapshot>()
                    .Where(x => x.DatasetId == datasetId)
                    .OrderBy(x => x.DatasetId).ThenBy(x => x.ClientId).ThenBy(x => x.LoanId)
                    .ToArray();
            }
        }

        public void CreateMany(
            IEnumerable<LoanSnapshot> items)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                PostgresCopyExecutor.ExecuteWrite(
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
