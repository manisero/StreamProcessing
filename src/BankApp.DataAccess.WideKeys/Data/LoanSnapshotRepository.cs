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
        private readonly bool _hasPk;
        private readonly bool _hasFk;

        public LoanSnapshotRepository(
            string connectionString,
            bool readUsingDapper = false,
            bool hasPk = true,
            bool hasFk = true)
        {
            _connectionString = connectionString;
            _readUsingDapper = readUsingDapper;
            _hasPk = hasPk;
            _hasFk = hasFk;
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
            if (_hasFk)
            {
                DatabaseManager.DropFk<LoanSnapshot, ClientSnapshot>(
                    _connectionString,
                    nameof(LoanSnapshot.DatasetId), nameof(LoanSnapshot.ClientId));
            }

            if (_hasPk)
            {
                DatabaseManager.DropPk<LoanSnapshot>(
                    _connectionString);
            }
        }

        public void RestoreConstraints()
        {
            if (_hasPk)
            {
                DatabaseManager.CreatePk<LoanSnapshot>(
                    _connectionString,
                    nameof(LoanSnapshot.DatasetId), nameof(LoanSnapshot.ClientId), nameof(LoanSnapshot.LoanId));
            }

            if (_hasFk)
            {
                DatabaseManager.CreateFk<LoanSnapshot, ClientSnapshot>(
                    _connectionString,
                    nameof(LoanSnapshot.DatasetId), nameof(LoanSnapshot.ClientId));
            }
        }
    }
}
