using System.Linq;
using BankApp.Domain.WideKeys.Tasks;

namespace BankApp.DataAccess.WideKeys.Tasks
{
    public class ClientLoansCalculationRepository
    {
        private readonly string _connectionString;

        public ClientLoansCalculationRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public short? GetMaxId()
        {
            using (var context = new EfContext(_connectionString))
            {
                return context.Set<ClientLoansCalculation>().Max(x => (short?)x.DatasetId);
            }
        }

        public ClientLoansCalculation Create(
            ClientLoansCalculation item)
        {
            using (var context = new EfContext(_connectionString))
            {
                context.Set<ClientLoansCalculation>().Add(item);
                context.SaveChanges();

                return item;
            }
        }
    }
}
