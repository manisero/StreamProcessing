using System.Collections.Generic;
using BankApp.Domain.WideKeys.Data;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp.DataAccess.WideKeys.Data
{
    public class ClientSnapshotRepository
    {
        protected readonly string ConnectionString;

        public ClientSnapshotRepository(
            string connectionString)
        {
            ConnectionString = connectionString;
        }

        public virtual void CreateMany(
            IEnumerable<ClientSnapshot> items)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                PostgresCopyExecutor.ExecuteWrite(
                    connection,
                    items,
                    ClientSnapshot.ColumnMapping);
            }
        }
    }

    public class ClientSnapshotRepositoryWithSchema : ClientSnapshotRepository
    {
        private readonly DatabaseManager _databaseManager;
        private readonly bool _hasPk;
        private readonly bool _hasFk;

        public ClientSnapshotRepositoryWithSchema(
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
                _databaseManager.DropFk<ClientSnapshot, Dataset>(
                    nameof(ClientSnapshot.DatasetId));

            if (_hasPk)
                _databaseManager.DropPk<ClientSnapshot>();
        }

        public void RestoreConstraints()
        {
            if (_hasPk)
                _databaseManager.CreatePk<ClientSnapshot>(
                    nameof(ClientSnapshot.DatasetId),
                    nameof(ClientSnapshot.ClientId));

            if (_hasFk)
                _databaseManager.CreateFk<ClientSnapshot, Dataset>(
                    nameof(ClientSnapshot.DatasetId));
        }
    }
}
