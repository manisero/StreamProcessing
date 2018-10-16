using System;
using System.Collections.Generic;
using BankApp.Domain;
using Npgsql;
using NpgsqlTypes;
using StreamProcessing.Utils.DatabaseAccess;

namespace BankApp.DataAccess
{
    public class ClientTotalLoanRepository
    {
        private static readonly Dictionary<string, Action<NpgsqlBinaryImporter, ClientTotalLoan>> ColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, ClientTotalLoan>>
            {
                [nameof(ClientTotalLoan.ClientLoansCalculationId)] = (writer, x) => writer.Write(x.ClientLoansCalculationId, NpgsqlDbType.Integer),
                [nameof(ClientTotalLoan.ClientId)] = (writer, x) => writer.Write(x.ClientId, NpgsqlDbType.Integer),
                [nameof(ClientTotalLoan.TotalLoan)] = (writer, x) => writer.Write(x.TotalLoan, NpgsqlDbType.Numeric)
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
