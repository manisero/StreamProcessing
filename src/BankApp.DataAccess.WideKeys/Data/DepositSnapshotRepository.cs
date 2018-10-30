using System.Collections.Generic;
using System.Linq;
using BankApp.Domain.WideKeys;
using BankApp.Domain.WideKeys.Data;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp.DataAccess.WideKeys.Data
{
    public class DepositSnapshotRepository
    {
        private readonly string _connectionString;

        public DepositSnapshotRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public ICollection<DepositSnapshot> GetForDataset(
            short datasetId)
        {
            using (var context = new EfContext(_connectionString))
            {
                return context
                    .Set<DepositSnapshot>()
                    .Where(x => x.DatasetId == datasetId)
                    .OrderBy(x => x.DatasetId).ThenBy(x => x.ClientId).ThenBy(x => x.DepositId)
                    .ToArray();
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
    }

    public class DepositSnapshotRepositoryWithSchema : DepositSnapshotRepository
    {
        private readonly DatabaseManager _databaseManager;
        private readonly bool _hasPk;
        private readonly bool _hasFk;

        public DepositSnapshotRepositoryWithSchema(
            string connectionString,
            DatabaseManager databaseManager,
            bool hasPk = true,
            bool hasFk = false)
            : base(connectionString)
        {
            _databaseManager = databaseManager;
            _hasPk = hasPk;
            _hasFk = hasFk;
        }

        public void DropConstraints()
        {
            if (_hasFk)
                _databaseManager.DropFk<DepositSnapshot, ClientSnapshot>(
                    nameof(DepositSnapshot.DatasetId),
                    nameof(DepositSnapshot.ClientId));

            if (_hasPk)
                _databaseManager.DropPk<DepositSnapshot>();
        }

        public void RestoreConstraints()
        {
            if (_hasPk)
                _databaseManager.CreatePk<DepositSnapshot>(
                    nameof(DepositSnapshot.DatasetId),
                    nameof(DepositSnapshot.ClientId),
                    nameof(DepositSnapshot.DepositId));

            if (_hasFk)
                _databaseManager.CreateFk<DepositSnapshot, ClientSnapshot>(
                    nameof(DepositSnapshot.DatasetId),
                    nameof(DepositSnapshot.ClientId));
        }
    }
}
