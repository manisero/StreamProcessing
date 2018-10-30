using System.Linq;
using BankApp.Domain.WideKeys;
using BankApp.Domain.WideKeys.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace BankApp.DataAccess.WideKeys.Tasks
{
    public class ClientLoansCalculationRepository
    {
        protected readonly string ConnectionString;

        public ClientLoansCalculationRepository(
            string connectionString)
        {
            ConnectionString = connectionString;
        }

        public virtual short? GetMaxId()
        {
            using (var context = new EfContext(ConnectionString))
            {
                return context.Set<ClientLoansCalculation>().Max(x => (short?)x.ClientLoansCalculationId);
            }
        }

        public virtual ClientLoansCalculation Create(
            ClientLoansCalculation item)
        {
            using (var context = new EfContext(ConnectionString))
            {
                context.Set<ClientLoansCalculation>().Add(item);
                context.SaveChanges();

                return item;
            }
        }

        public virtual void Delete(
            short id)
        {
            var clientTotalLoanSql = $@"
DELETE FROM ""{nameof(ClientTotalLoan)}""
WHERE ""{nameof(ClientTotalLoan.ClientLoansCalculationId)}"" = @ClientLoansCalculationId";

            using (var context = new EfContext(ConnectionString))
            {
                context.Database.GetDbConnection().Execute(clientTotalLoanSql, new { ClientLoansCalculationId = id });

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
