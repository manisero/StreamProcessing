using BankApp.Domain.WideKeys;
using BankApp.Domain.WideKeys.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BankApp.DataAccess.WideKeys.Tasks
{
    public class MaxLossCalculationRepository
    {
        private readonly string _connectionString;

        public MaxLossCalculationRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public MaxLossCalculation Create(
            MaxLossCalculation item)
        {
            using (var context = new EfContext(_connectionString))
            {
                context.Set<MaxLossCalculation>().Add(item);
                context.SaveChanges();

                return item;
            }
        }

        public MaxLossCalculation Update(
            MaxLossCalculation item)
        {
            using (var context = new EfContext(_connectionString))
            {
                var entry = context.Set<MaxLossCalculation>().Attach(item);
                entry.State = EntityState.Modified;
                context.SaveChanges();

                return item;
            }
        }
    }
}
