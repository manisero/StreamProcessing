using System.Collections.Generic;
using System.Linq;
using BankApp.Domain.WideKeys;
using BankApp.Domain.WideKeys.Data;
using BankApp.Domain.WideKeys.Tasks;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp.DataAccess.Partitioned.Data
{
    public class DatasetRepository
    {
        private readonly string _connectionString;
        private readonly DatabaseManager _databaseManager;

        public DatasetRepository(
            string connectionString,
            DatabaseManager databaseManager)
        {
            _connectionString = connectionString;
            _databaseManager = databaseManager;
        }

        public ICollection<Dataset> GetAll()
        {
            using (var context = new EfContext(_connectionString))
            {
                return context.Set<Dataset>().ToArray();
            }
        }

        public short? GetMaxId()
        {
            using (var context = new EfContext(_connectionString))
            {
                return context.Set<Dataset>().Max(x => (short?)x.DatasetId);
            }
        }

        public Dataset Create(
            Dataset item)
        {
            using (var context = new EfContext(_connectionString))
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

        public void CreateMany(
            IEnumerable<Dataset> items)
        {
            foreach (var item in items)
            {
                Create(item);
            }
        }
    }
}
