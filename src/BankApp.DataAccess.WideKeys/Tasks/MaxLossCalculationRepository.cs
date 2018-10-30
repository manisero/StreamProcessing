using BankApp.Domain.WideKeys;
using BankApp.Domain.WideKeys.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BankApp.DataAccess.WideKeys.Tasks
{
    public class MaxLossCalculationRepository
    {
        protected readonly string ConnectionString;

        public MaxLossCalculationRepository(
            string connectionString)
        {
            ConnectionString = connectionString;
        }

        public virtual MaxLossCalculation Create(
            MaxLossCalculation item)
        {
            using (var context = new EfContext(ConnectionString))
            {
                context.Set<MaxLossCalculation>().Add(item);
                context.SaveChanges();

                return item;
            }
        }

        public virtual MaxLossCalculation Update(
            MaxLossCalculation item)
        {
            using (var context = new EfContext(ConnectionString))
            {
                var entry = context.Set<MaxLossCalculation>().Attach(item);
                entry.State = EntityState.Modified;
                context.SaveChanges();

                return item;
            }
        }
    }
}
