using System.Collections.Generic;
using System.Linq;
using BankApp.Domain.WideKeys;
using BankApp.Domain.WideKeys.Data;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp.DataAccess.WideKeys.Data
{
    public class DatasetRepository
    {
        protected readonly string ConnectionString;

        public DatasetRepository(
            string connectionString)
        {
            ConnectionString = connectionString;
        }
        
        public virtual ICollection<Dataset> GetAll()
        {
            using (var context = new EfContext(ConnectionString))
            {
                return context.Set<Dataset>().ToArray();
            }
        }

        public virtual short? GetMaxId()
        {
            using (var context = new EfContext(ConnectionString))
            {
                return context.Set<Dataset>().Max(x => (short?)x.DatasetId);
            }
        }

        public virtual void CreateMany(
            IEnumerable<Dataset> items)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                PostgresCopyExecutor.ExecuteWrite(
                    connection,
                    items,
                    Dataset.ColumnMapping);
            }
        }
    }
}
