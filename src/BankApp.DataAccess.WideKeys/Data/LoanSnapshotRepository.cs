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
        protected readonly string ConnectionString;
        protected readonly bool ReadUsingDapper;

        public LoanSnapshotRepository(
            string connectionString,
            bool readUsingDapper = false)
        {
            ConnectionString = connectionString;
            ReadUsingDapper = readUsingDapper;
        }

        public ICollection<LoanSnapshot> GetAll()
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                return PostgresCopyExecutor.ExecuteRead(
                    connection,
                    LoanSnapshot.RowReader);
            }
        }

        public int CountInDataset(
            short datasetId)
        {
            using (var context = new EfContext(ConnectionString))
            {
                return context
                    .Set<LoanSnapshot>()
                    .Count(x => x.DatasetId == datasetId);
            }
        }

        public ICollection<LoanSnapshot> GetForDataset(
            short datasetId)
        {
            return ReadUsingDapper
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

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                return connection
                    .Query<LoanSnapshot>(sql, new { DatasetId = datasetId })
                    .AsList();
            }
        }

        private ICollection<LoanSnapshot> GetForDataset_Ef(
            short datasetId)
        {
            using (var context = new EfContext(ConnectionString))
            {
                return context
                    .Set<LoanSnapshot>()
                    .Where(x => x.DatasetId == datasetId)
                    .OrderBy(x => x.DatasetId).ThenBy(x => x.ClientId).ThenBy(x => x.LoanId)
                    .ToArray();
            }
        }

        public ICollection<LoanSnapshot> GetBatchForDataset(
            short datasetId,
            int skip,
            int take)
        {
            using (var context = new EfContext(ConnectionString))
            {
                return context
                    .Set<LoanSnapshot>()
                    .Where(x => x.DatasetId == datasetId)
                    .OrderBy(x => x.DatasetId).ThenBy(x => x.ClientId).ThenBy(x => x.LoanId)
                    .Skip(skip).Take(skip)
                    .ToArray();
            }
        }

        public void CreateMany(
            IEnumerable<LoanSnapshot> items)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                PostgresCopyExecutor.ExecuteWrite(
                    connection,
                    items,
                    LoanSnapshot.ColumnMapping);
            }
        }
    }

    public class LoanSnapshotRepositoryWithSchema : LoanSnapshotRepository
    {
        private readonly bool _hasPk;
        private readonly bool _hasFk;

        public LoanSnapshotRepositoryWithSchema(
            string connectionString,
            bool readUsingDapper = false,
            bool hasPk = true,
            bool hasFk = false)
            : base(connectionString, readUsingDapper)
        {
            _hasPk = hasPk;
            _hasFk = hasFk;
        }

        public void DropConstraints()
        {
            if (_hasFk)
            {
                DatabaseManager.DropFk<LoanSnapshot, ClientSnapshot>(
                    ConnectionString,
                    nameof(LoanSnapshot.DatasetId), nameof(LoanSnapshot.ClientId));
            }

            if (_hasPk)
            {
                DatabaseManager.DropPk<LoanSnapshot>(
                    ConnectionString);
            }
        }

        public void RestoreConstraints()
        {
            if (_hasPk)
            {
                DatabaseManager.CreatePk<LoanSnapshot>(
                    ConnectionString,
                    nameof(LoanSnapshot.DatasetId), nameof(LoanSnapshot.ClientId), nameof(LoanSnapshot.LoanId));
            }

            if (_hasFk)
            {
                DatabaseManager.CreateFk<LoanSnapshot, ClientSnapshot>(
                    ConnectionString,
                    nameof(LoanSnapshot.DatasetId), nameof(LoanSnapshot.ClientId));
            }
        }
    }
}
