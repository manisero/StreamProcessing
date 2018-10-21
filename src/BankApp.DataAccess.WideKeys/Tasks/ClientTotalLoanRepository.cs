using System.Collections.Generic;
using BankApp.Domain.WideKeys.Tasks;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp.DataAccess.WideKeys.Tasks
{
    public class ClientTotalLoanRepository
    {
        private readonly string _connectionString;

        public ClientTotalLoanRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateMany(
            IEnumerable<ClientTotalLoan> items)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                PostgresCopyExecutor.Execute(
                    connection,
                    items,
                    ClientTotalLoan.ColumnMapping);
            }
        }
    }
}
