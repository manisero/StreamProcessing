using BankApp.Domain;
using Microsoft.EntityFrameworkCore;

namespace BankApp.DataAccess
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
                .UseNpgsql(_connectionString);
        }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dataset>().Property(x => x.Date).HasColumnType("date");
            modelBuilder.Entity<ClientSnapshot>();
            modelBuilder.Entity<LoanSnapshot>();
        }
    }
}
