using System.Collections.Generic;
using BankApp8.Common.DataAccess;
using BankApp8.Common.Domain;

namespace BankApp8.Init.DbSeeding.DatasetPopulation
{
    public static class DatasetPopulator
    {
        public static void Populate(
            short datasetId,
            string connectionString,
            int clientsCount,
            int loansPerClient)
        {
            new ClientSnapshotRepository(connectionString)
                .CreateMany(GetClients(datasetId, clientsCount));

            new LoanSnapshotRepository(connectionString)
                .CreateMany(GetLoans(datasetId, clientsCount, loansPerClient));
        }

        private static IEnumerable<ClientSnapshot> GetClients(
            short datasetId,
            int clientsCount)
        {
            for (var clientId = 1; clientId <= clientsCount; clientId++)
            {
                yield return new ClientSnapshot
                {
                    DatasetId = datasetId,
                    ClientId = clientId
                };
            }
        }

        private static IEnumerable<LoanSnapshot> GetLoans(
            short datasetId,
            int clientsCount,
            int loansPerClient)
        {
            var nextLoanId = 1;

            for (var clientId = 1; clientId <= clientsCount; clientId++)
            {
                for (var loanIndex = 0; loanIndex < loansPerClient; loanIndex++)
                {
                    yield return new LoanSnapshot
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
