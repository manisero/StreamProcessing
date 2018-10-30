using System.Collections.Generic;
using BankApp.Domain.WideKeys;
using BankApp.Domain.WideKeys.Tasks;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp.DataAccess.WideKeys.Tasks
{
    public class ClientTotalLoanRepository
    {
        protected readonly string _connectionString;
        private readonly bool _createUsingCopy;

        public ClientTotalLoanRepository(
            string connectionString,
            bool createUsingCopy = true)
        {
            _connectionString = connectionString;
            _createUsingCopy = createUsingCopy;
        }

        public virtual void CreateMany(
            IEnumerable<ClientTotalLoan> items)
        {
            if (_createUsingCopy)
            {
                CreateMany_Copy(items);
            }
            else
            {
                CreateMany_Ef(items);
            }
        }

        private void CreateMany_Copy(
            IEnumerable<ClientTotalLoan> items)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                PostgresCopyExecutor.ExecuteWrite(
                    connection,
                    items,
                    ClientTotalLoan.ColumnMapping);
            }
        }

        private void CreateMany_Ef(
            IEnumerable<ClientTotalLoan> items)
        {
            using (var context = new EfContext(_connectionString))
            {
                context.Set<ClientTotalLoan>().AddRange(items);
                context.SaveChanges();
            }
        }
    }
}
