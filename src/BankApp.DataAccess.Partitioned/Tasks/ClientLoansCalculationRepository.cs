using System.Collections.Generic;
using System.Linq;
using BankApp.Domain.WideKeys;
using BankApp.Domain.WideKeys.Tasks;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp.DataAccess.Partitioned.Tasks
{
    public class ClientLoansCalculationRepository
    {
        private readonly string _connectionString;
        private readonly DatabaseManager _databaseManager;

        public ClientLoansCalculationRepository(
            string connectionString,
            DatabaseManager databaseManager)
        {
            _connectionString = connectionString;
            _databaseManager = databaseManager;
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
            }

            _databaseManager.CreatePartition<ClientTotalLoan>(
                item.ClientLoansCalculationId,
                nameof(ClientTotalLoan.ClientLoansCalculationId),
                nameof(ClientTotalLoan.ClientId));

            return item;
        }

        public void SaveClientTotalLoans(
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
    }
}
