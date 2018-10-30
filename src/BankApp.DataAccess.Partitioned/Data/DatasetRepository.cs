using System.Collections.Generic;
using BankApp.Domain.WideKeys;
using BankApp.Domain.WideKeys.Data;
using DataProcessing.Utils.DatabaseAccess;

namespace BankApp.DataAccess.Partitioned.Data
{
    public class DatasetRepository : WideKeys.Data.DatasetRepository
    {
        private readonly DatabaseManager _databaseManager;

        public DatasetRepository(
            string connectionString,
            DatabaseManager databaseManager)
            : base(connectionString)
        {
            _databaseManager = databaseManager;
        }

        public Dataset Create(
            Dataset item)
        {
            using (var context = new EfContext(ConnectionString))
            {
                context.Set<Dataset>().Add(item);
                context.SaveChanges();
            }

            _databaseManager.CreatePartition<ClientSnapshot>(
                item.DatasetId,
                nameof(ClientSnapshot.DatasetId),
                nameof(ClientSnapshot.ClientId));

            _databaseManager.CreatePartition<DepositSnapshot>(
                item.DatasetId,
                nameof(DepositSnapshot.DatasetId),
                nameof(DepositSnapshot.ClientId),
                nameof(DepositSnapshot.DepositId));

            _databaseManager.CreatePartition<LoanSnapshot>(
                item.DatasetId,
                nameof(LoanSnapshot.DatasetId),
                nameof(LoanSnapshot.ClientId),
                nameof(LoanSnapshot.LoanId));

            return item;
        }

        public override void CreateMany(
            IEnumerable<Dataset> items)
        {
            foreach (var item in items)
            {
                Create(item);
            }
        }
    }
}
