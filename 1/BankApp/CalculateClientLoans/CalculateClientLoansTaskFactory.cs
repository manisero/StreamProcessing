﻿using System;
using System.Collections.Generic;
using System.Linq;
using BankApp.DataAccess;
using BankApp.Domain;
using Manisero.Navvy;
using Manisero.Navvy.PipelineProcessing;
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

            return new TaskDefinition(
                $"{nameof(CalculateClientLoans)}_{clientLoansCalculation.ClientLoansCalculationId}",
                PipelineTaskStep.Builder<DatasetToProcess>(nameof(CalculateClientLoans))
                    .WithInput(LoadDatasetAsEnumerable(datasetId), 1)
                    .WithBlock(
                        "Calculate",
                        x =>
                        {
                            foreach (var client in x.Dataset.Clients)
                            {
                                x.ClientLoans.Add(client.ClientId, client.Loans.Sum(l => l.Value));
                            }
                        })
                    .WithBlock(
                        "SaveClientTotalLoans",
                        x => SaveClientTotalLoans(x, clientLoansCalculation.ClientLoansCalculationId))
                    .Build());
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

        private IEnumerable<DatasetToProcess> LoadDatasetAsEnumerable(
            int datasetId)
        {
            using (var context = _efContextFactory())
            {
                var dataset = context
                    .Set<Dataset>()
                    .Include(x => x.Clients).ThenInclude(x => x.Loans)
                    .Single(x => x.DatasetId == datasetId);

                yield return new DatasetToProcess { Dataset = dataset };
            }
        }

        private void SaveClientTotalLoans(
            DatasetToProcess datasetToProcess,
            int clientLoansCalculationId)
        {
            using (var context = _efContextFactory())
            {
                var clientTotalLoans = datasetToProcess.ClientLoans.Select(
                    x => new ClientTotalLoan
                    {
                        ClientLoansCalculationId = clientLoansCalculationId,
                        ClientId = x.Key,
                        TotalLoan = x.Value
                    });

                context.Set<ClientTotalLoan>().AddRange(clientTotalLoans);
                context.SaveChanges();
            }
        }
    }
}