using System.Linq;
using BankApp.Domain.WideKeys.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;

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
                return context.Set<ClientLoansCalculation>().Max(x => (short?)x.ClientLoansCalculationId);
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

        public void Delete(
            short id)
        {
            var clientTotalLoanSql = $@"
DELETE FROM ""{nameof(ClientTotalLoan)}""
WHERE ""{nameof(ClientTotalLoan.ClientLoansCalculationId)}"" = @ClientLoansCalculationId";

            using (var context = new EfContext(_connectionString))
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
