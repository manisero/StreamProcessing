using BankApp.Domain.WideKeys.Data;
using BankApp.Domain.WideKeys.Tasks;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace BankApp3.Common.DataAccess
{
    internal class EfContext : DbContext
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
            modelBuilder.Entity<ClientSnapshot>().HasKey(x => new { x.DatasetId, x.ClientId });
            modelBuilder.Entity<DepositSnapshot>().HasKey(x => new { x.DatasetId, x.ClientId, x.DepositId });
            modelBuilder.Entity<LoanSnapshot>().HasKey(x => new { x.DatasetId, x.ClientId, x.LoanId });
            modelBuilder.Entity<TotalLoanCalculation>();
            modelBuilder.Entity<ClientLoansCalculation>();
            modelBuilder.Entity<ClientTotalLoan>().HasKey(x => new { x.ClientLoansCalculationId, x.ClientId });
        }
    }
}
