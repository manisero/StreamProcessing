using System.Collections.Generic;
using System.Linq;
using BankApp.Domain.WideKeys;
using BankApp.Domain.WideKeys.Data;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp.DataAccess.Partitioned.Data
{
    public class DatasetRepository
    {
        private readonly string _connectionString;

        public DatasetRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public ICollection<Dataset> GetAll()
        {
            using (var context = new EfContext(_connectionString))
            {
                return context.Set<Dataset>().ToArray();
            }
        }

        public short? GetMaxId()
        {
            using (var context = new EfContext(_connectionString))
            {
                return context.Set<Dataset>().Max(x => (short?)x.DatasetId);
            }
        }

        public void CreateMany(
            IEnumerable<Dataset> items)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                PostgresCopyExecutor.ExecuteWrite(
                    connection,
                    items,
                    Dataset.ColumnMapping);
            }
        }
    }
}
