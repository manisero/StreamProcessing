using Microsoft.EntityFrameworkCore;

namespace DataProcessing.Utils.DatabaseAccess
{
    public static class DatabaseManager
    {
        private class EmptyEfContext : DbContext
        {
            private readonly string _connectionString;

            public EmptyEfContext(
                string connectionString)
            {
                _connectionString = connectionString;
            }

            protected override void OnConfiguring(
                DbContextOptionsBuilder optionsBuilder) 
                => optionsBuilder.UseNpgsql(_connectionString);
        }

        public static bool EnsureCreated(
            string connectionString)
        {
            using (var efContext = new EmptyEfContext(connectionString))
            {
                return efContext.Database.EnsureCreated();
            }
        }

        public static bool EnsureDeleted(
            string connectionString)
        {
            using (var efContext = new EmptyEfContext(connectionString))
            {
                return efContext.Database.EnsureDeleted();
            }
        }
    }
}
