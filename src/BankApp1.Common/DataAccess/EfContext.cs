using BankApp.Domain.SurrogateKeys;
using BankApp.Domain.SurrogateKeys.Data;
using BankApp.Domain.SurrogateKeys.Tasks;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace BankApp1.Common.DataAccess
{
    public class EfContext : DbContext
    {
        private readonly string _connectionString;

        public EfContext(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
            {
                return;
            }

            optionsBuilder
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .UseNpgsql(_connectionString);
        }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            modelBuilder.ForNpgsqlUseIdentityAlwaysColumns();

            modelBuilder.Entity<Dataset>().Property(x => x.Date).HasColumnType(nameof(NpgsqlDbType.Date));
            modelBuilder.Entity<ClientSnapshot>();
            modelBuilder.Entity<DepositSnapshot>();
            modelBuilder.Entity<LoanSnapshot>();
            modelBuilder.Entity<TotalLoanCalculation>();
            modelBuilder.Entity<ClientLoansCalculation>();
            modelBuilder.Entity<ClientTotalLoan>();
        }
    }
}
