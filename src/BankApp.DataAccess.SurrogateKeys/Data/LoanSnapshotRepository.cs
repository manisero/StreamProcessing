using System.Collections.Generic;
using System.Linq;
using BankApp.Domain.SurrogateKeys;
using BankApp.Domain.SurrogateKeys.Data;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp.DataAccess.SurrogateKeys.Data
{
    public class LoanSnapshotRepository
    {
        private readonly string _connectionString;

        public LoanSnapshotRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public ICollection<LoanSnapshot> GetForDataset(
            int datasetId)
        {
            using (var context = new EfContext(_connectionString))
            {
                return context
                    .Set<LoanSnapshot>()
                    .Where(x => x.Client.DatasetId == datasetId)
                    .ToArray();
            }
        }

        public void CreateMany(
            IEnumerable<LoanSnapshot> items)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                PostgresCopyExecutor.ExecuteWrite(
                    connection,
                    items,
                    LoanSnapshot.ColumnMapping);
            }
        }
    }
}
