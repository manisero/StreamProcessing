using BankApp.Domain.WideKeys;
using BankApp.Domain.WideKeys.Tasks;
using DataProcessing.Utils.DatabaseAccess;

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
    }
}
