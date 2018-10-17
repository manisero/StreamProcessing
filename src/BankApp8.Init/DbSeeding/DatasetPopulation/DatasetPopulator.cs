using System;
using System.Collections.Generic;
using BankApp8.Common.Domain;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;
using NpgsqlTypes;

namespace BankApp8.Init.DbSeeding.DatasetPopulation
{
    public static class DatasetPopulator
    {
        private static readonly Dictionary<string, Action<NpgsqlBinaryImporter, Client>> ClientColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, Client>>
            {
                [nameof(Client.DatasetId)] = (writer, x) => writer.Write(x.DatasetId, NpgsqlDbType.Smallint),
                [nameof(Client.ClientId)] = (writer, x) => writer.Write(x.ClientId, NpgsqlDbType.Integer)
            };

        private static readonly Dictionary<string, Action<NpgsqlBinaryImporter, Loan>> LoanColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, Loan>>
            {
                [nameof(Loan.DatasetId)] = (writer, x) => writer.Write(x.DatasetId, NpgsqlDbType.Smallint),
                [nameof(Loan.ClientId)] = (writer, x) => writer.Write(x.ClientId, NpgsqlDbType.Integer),
                [nameof(Loan.LoanId)] = (writer, x) => writer.Write(x.LoanId, NpgsqlDbType.Integer),
                [nameof(Loan.Value)] = (writer, x) => writer.Write(x.Value, NpgsqlDbType.Numeric)
            };

        public static void Populate(
            short datasetId,
            string connectionString,
            int clientsCount,
            int loansPerClient)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                PostgresCopyExecutor.Execute(
                    connection,
                    GetClients(datasetId, clientsCount),
                    ClientColumnMapping);

                PostgresCopyExecutor.Execute(
                    connection,
                    GetLoans(datasetId, clientsCount, loansPerClient),
                    LoanColumnMapping);
            }
        }

        private static IEnumerable<Client> GetClients(
            short datasetId,
            int clientsCount)
        {
            for (var clientId = 1; clientId <= clientsCount; clientId++)
            {
                yield return new Client
                {
                    DatasetId = datasetId,
                    ClientId = clientId
                };
            }
        }

        private static IEnumerable<Loan> GetLoans(
            short datasetId,
            int clientsCount,
            int loansPerClient)
        {
            var nextLoanId = 1;

            for (var clientId = 1; clientId <= clientsCount; clientId++)
            {
                for (var loanIndex = 0; loanIndex < loansPerClient; loanIndex++)
                {
                    yield return new Loan
                    {
                        DatasetId = datasetId,
                        ClientId = clientId,
                        LoanId = nextLoanId,
                        Value = 1m // TODO: Randomize
                    };

                    nextLoanId++;
                }
            }
        }
    }
}
