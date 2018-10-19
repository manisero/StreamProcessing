using System;
using System.Collections.Generic;
using BankApp1.Common.Domain;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp1.Common.DataAccess
{
    public class ClientTotalLoanRepository
    {
        private static readonly Dictionary<string, Action<NpgsqlBinaryImporter, ClientTotalLoan>> ColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, ClientTotalLoan>>
            {
                [nameof(ClientTotalLoan.ClientLoansCalculationId)] = (writer, x) => writer.Write(x.ClientLoansCalculationId),
                [nameof(ClientTotalLoan.ClientId)] = (writer, x) => writer.Write(x.ClientId),
                [nameof(ClientTotalLoan.TotalLoan)] = (writer, x) => writer.Write(x.TotalLoan)
            };

        private readonly string _connectionString;
        private readonly bool _createUsingCopy;

        public ClientTotalLoanRepository(
            string connectionString,
            bool createUsingCopy = true)
        {
            _connectionString = connectionString;
            _createUsingCopy = createUsingCopy;
        }

        public void CreateMany(
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
                PostgresCopyExecutor.Execute(
                    connection,
                    items,
                    ColumnMapping);
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
