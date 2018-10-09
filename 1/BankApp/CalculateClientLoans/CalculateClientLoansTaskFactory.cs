using System;
using System.Linq;
using BankApp.DataAccess;
using BankApp.Domain;
using Manisero.Navvy;
using Manisero.Navvy.BasicProcessing;
using Microsoft.EntityFrameworkCore;

namespace BankApp.CalculateClientLoans
{
    public class CalculateClientLoansTaskFactory
    {
        private readonly Func<EfContext> _efContextFactory;

        public CalculateClientLoansTaskFactory(
            Func<EfContext> efContextFactory)
        {
            _efContextFactory = efContextFactory;
        }

        public TaskDefinition Create(
            int datasetId)
        {
            var clientLoansCalculation = CreateClientLoansCalculation(datasetId);
            var datasetToProcess = new DatasetToProcess();

            return new TaskDefinition(
                $"{nameof(CalculateClientLoans)}_{clientLoansCalculation.ClientLoansCalculationId}",
                new BasicTaskStep(
                    "LoadDataset",
                    () =>
                    {
                        using (var context = _efContextFactory())
                        {
                            datasetToProcess.Dataset = context
                                .Set<Dataset>()
                                .Include(x => x.Clients).ThenInclude(x => x.Loans)
                                .Single(x => x.DatasetId == datasetId);
                        }
                    }),
                new BasicTaskStep(
                    "CalculateTotalLoans",
                    () =>
                    {
                        foreach (var client in datasetToProcess.Dataset.Clients)
                        {
                            datasetToProcess.ClientLoans.Add(client.ClientId, client.Loans.Sum(l => l.Value));
                        }
                    }),
                new BasicTaskStep(
                    "SaveClientTotalLoans",
                    () =>
                    {
                        using (var context = _efContextFactory())
                        {
                            var clientTotalLoans = datasetToProcess.ClientLoans.Select(
                                x => new ClientTotalLoan
                                {
                                    ClientLoansCalculationId = clientLoansCalculation.ClientLoansCalculationId,
                                    ClientId = x.Key,
                                    TotalLoan = x.Value
                                });

                            context.Set<ClientTotalLoan>().AddRange(clientTotalLoans);
                            context.SaveChanges();
                        }
                    }));
        }

        private ClientLoansCalculation CreateClientLoansCalculation(
            int datasetId)
        {
            using (var context = _efContextFactory())
            {
                var clientLoansCalculation = new ClientLoansCalculation
                {
                    DatasetId = datasetId
                };

                context.Set<ClientLoansCalculation>().Add(clientLoansCalculation);
                context.SaveChanges();

                return clientLoansCalculation;
            }
        }
    }
}
