using System;
using System.Linq;
using BankApp1.Common.DataAccess;
using BankApp1.Common.Domain;
using Manisero.Navvy;
using Manisero.Navvy.BasicProcessing;
using Microsoft.EntityFrameworkCore;

namespace BankApp1.Main.ClientLoansCalculationTask
{
    public class ClientLoansCalculationTaskFactory
    {
        private readonly Func<EfContext> _efContextFactory;

        public ClientLoansCalculationTaskFactory(
            Func<EfContext> efContextFactory)
        {
            _efContextFactory = efContextFactory;
        }

        public TaskDefinition Create(
            int datasetId)
        {
            var clientLoansCalculation = CreateClientLoansCalculation(datasetId);
            var state = new ClientLoansCalculationState();

            return new TaskDefinition(
                $"{nameof(ClientLoansCalculation)}_{clientLoansCalculation.ClientLoansCalculationId}",
                new BasicTaskStep(
                    "LoadDataset",
                    () =>
                    {
                        using (var context = _efContextFactory())
                        {
                            state.Dataset = context
                                .Set<Dataset>()
                                .Include(x => x.Clients).ThenInclude(x => x.Loans)
                                .Single(x => x.DatasetId == datasetId);
                        }
                    }),
                new BasicTaskStep(
                    "CalculateTotalLoans",
                    () =>
                    {
                        foreach (var client in state.Dataset.Clients)
                        {
                            state.ClientLoans.Add(
                                client.ClientId,
                                client.Loans.Sum(l => l.Value));
                        }
                    }),
                new BasicTaskStep(
                    "SaveClientTotalLoans",
                    () =>
                    {
                        using (var context = _efContextFactory())
                        {
                            var clientTotalLoans = state.ClientLoans.Select(
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
