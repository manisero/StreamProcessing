using System;
using System.Collections.Generic;
using BankApp.Domain.WideKeys;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp3.Common.DataAccess
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
                    ColumnMapping);
            }
        }
    }
}
