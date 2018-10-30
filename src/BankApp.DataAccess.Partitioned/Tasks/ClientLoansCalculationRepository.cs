using BankApp.Domain.WideKeys;
using BankApp.Domain.WideKeys.Tasks;
using Dapper;
using DataProcessing.Utils.DatabaseAccess;
using Microsoft.EntityFrameworkCore;

namespace BankApp.DataAccess.Partitioned.Tasks
{
    public class ClientLoansCalculationRepository : WideKeys.Tasks.ClientLoansCalculationRepository
    {
        private readonly DatabaseManager _databaseManager;

        public ClientLoansCalculationRepository(
            string connectionString,
            DatabaseManager databaseManager)
            : base(connectionString)
        {
            _databaseManager = databaseManager;
        }

        public override ClientLoansCalculation Create(
            ClientLoansCalculation item)
        {
            using (var context = new EfContext(ConnectionString))
            {
                context.Set<ClientLoansCalculation>().Add(item);
                context.SaveChanges();
            }

            _databaseManager.CreatePartition<ClientTotalLoan>(
                item.ClientLoansCalculationId,
                nameof(ClientTotalLoan.ClientLoansCalculationId),
                nameof(ClientTotalLoan.ClientId));

            return item;
        }

        public override void Delete(
            short id)
        {
            using (var context = new EfContext(ConnectionString))
            {
                _databaseManager.DropPartition<ClientTotalLoan>(id);

                var entry = context.Entry(new ClientLoansCalculation
                {
                    ClientLoansCalculationId = id
                });

                entry.State = EntityState.Deleted;
                context.SaveChanges();
            }
        }
    }
}
