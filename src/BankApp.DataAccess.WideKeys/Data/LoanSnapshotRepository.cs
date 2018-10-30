using System.Collections.Generic;
using System.Linq;
using BankApp.Domain.WideKeys;
using BankApp.Domain.WideKeys.Data;
using Dapper;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp.DataAccess.WideKeys.Data
{
    public class LoanSnapshotRepository
    {
        protected readonly string _connectionString;
        private readonly bool _readUsingDapper;

        public LoanSnapshotRepository(
            string connectionString,
            bool readUsingDapper = false)
        {
            _connectionString = connectionString;
            _readUsingDapper = readUsingDapper;
        }

        public virtual ICollection<LoanSnapshot> GetAll()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return PostgresCopyExecutor.ExecuteRead(
                    connection,
                    LoanSnapshot.RowReader);
            }
        }

        public virtual int CountInDataset(
            short datasetId)
        {
            using (var context = new EfContext(_connectionString))
            {
                return context
                    .Set<LoanSnapshot>()
                    .Count(x => x.DatasetId == datasetId);
            }
        }

        public virtual ICollection<LoanSnapshot> GetForDataset(
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
SELECT *
FROM ""{nameof(LoanSnapshot)}""
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

        public virtual ICollection<LoanSnapshot> GetBatchForDataset(
            short datasetId,
            int skip,
            int take)
        {
            return _readUsingDapper
                ? GetBatchForDataset_Dapper(datasetId, skip, take)
                : GetBatchForDataset_Ef(datasetId, skip, take);
        }

        private ICollection<LoanSnapshot> GetBatchForDataset_Dapper(
            short datasetId,
            int skip,
            int take)
        {
            var sql = $@"
SELECT *
FROM ""{nameof(LoanSnapshot)}""
WHERE ""{nameof(LoanSnapshot.DatasetId)}"" = @DatasetId
ORDER BY ""{nameof(LoanSnapshot.DatasetId)}"", ""{nameof(LoanSnapshot.ClientId)}"", ""{nameof(LoanSnapshot.LoanId)}""
OFFSET @Skip ROWS LIMIT @Take";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return connection
                    .Query<LoanSnapshot>(sql, new { DatasetId = datasetId, Skip = skip, Take = take })
                    .AsList();
            }
        }

        private ICollection<LoanSnapshot> GetBatchForDataset_Ef(
            short datasetId,
            int skip,
            int take)
        {
            using (var context = new EfContext(_connectionString))
            {
                return context
                    .Set<LoanSnapshot>()
                    .Where(x => x.DatasetId == datasetId)
                    .OrderBy(x => x.DatasetId).ThenBy(x => x.ClientId).ThenBy(x => x.LoanId)
                    .Skip(skip).Take(take)
                    .ToArray();
            }
        }

        public virtual void CreateMany(
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
    }

    public class LoanSnapshotRepositoryWithSchema : LoanSnapshotRepository
    {
        private readonly DatabaseManager _databaseManager;
        private readonly bool _hasPk;
        private readonly bool _hasFk;

        public LoanSnapshotRepositoryWithSchema(
            string connectionString,
            DatabaseManager databaseManager,
            bool readUsingDapper = false,
            bool hasPk = true,
            bool hasFk = false)
            : base(connectionString, readUsingDapper)
        {
            _databaseManager = databaseManager;
            _hasPk = hasPk;
            _hasFk = hasFk;
        }

        public void DropConstraints()
        {
            if (_hasFk)
                _databaseManager.DropFk<LoanSnapshot, ClientSnapshot>(
                    nameof(LoanSnapshot.DatasetId),
                    nameof(LoanSnapshot.ClientId));

            if (_hasPk)
                _databaseManager.DropPk<LoanSnapshot>();
        }

        public void RestoreConstraints()
        {
            if (_hasPk)
                _databaseManager.CreatePk<LoanSnapshot>(
                    nameof(LoanSnapshot.DatasetId),
                    nameof(LoanSnapshot.ClientId),
                    nameof(LoanSnapshot.LoanId));

            if (_hasFk)
                _databaseManager.CreateFk<LoanSnapshot, ClientSnapshot>(
                    nameof(LoanSnapshot.DatasetId),
                    nameof(LoanSnapshot.ClientId));
        }
    }
}
