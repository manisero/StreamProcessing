using BankApp.Domain.WideKeys.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BankApp.DataAccess.WideKeys.Tasks
{
    public class TotalLoanCalculationRepository
    {
        private readonly string _connectionString;

        public TotalLoanCalculationRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public TotalLoanCalculation Create(
            TotalLoanCalculation item)
        {
            using (var context = new EfContext(_connectionString))
            {
                context.Set<TotalLoanCalculation>().Add(item);
                context.SaveChanges();

                return item;
            }
        }

        public TotalLoanCalculation Update(
            TotalLoanCalculation item)
        {
            using (var context = new EfContext(_connectionString))
            {
                var entry = context.Entry(item);
                entry.State = EntityState.Modified;
                context.SaveChanges();

                return item;
            }
        }
    }
}
