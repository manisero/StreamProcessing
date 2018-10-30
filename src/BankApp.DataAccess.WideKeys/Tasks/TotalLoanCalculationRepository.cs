using BankApp.Domain.WideKeys;
using BankApp.Domain.WideKeys.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BankApp.DataAccess.WideKeys.Tasks
{
    public class TotalLoanCalculationRepository
    {
        protected readonly string ConnectionString;

        public TotalLoanCalculationRepository(
            string connectionString)
        {
            ConnectionString = connectionString;
        }

        public virtual TotalLoanCalculation Create(
            TotalLoanCalculation item)
        {
            using (var context = new EfContext(ConnectionString))
            {
                context.Set<TotalLoanCalculation>().Add(item);
                context.SaveChanges();

                return item;
            }
        }

        public virtual TotalLoanCalculation Update(
            TotalLoanCalculation item)
        {
            using (var context = new EfContext(ConnectionString))
            {
                var entry = context.Entry(item);
                entry.State = EntityState.Modified;
                context.SaveChanges();

                return item;
            }
        }
    }
}
