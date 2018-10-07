using System;
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
            return new TaskDefinition(
                nameof(CalculateClientLoans),
                PipelineTaskStep.Builder<Dataset>(nameof(CalculateClientLoans))
                    .WithInput(
                        LoadDataset(datasetId),
                        1)
                    .WithBlock(
                        nameof(CalculateClientLoans),
                        x =>
                        {
                            var a = 3;
                        })
                    .Build());
        }

        public IEnumerable<Dataset> LoadDataset(
            int datasetId)
        {
            using (var context = _efContextFactory())
            {
                yield return context
                    .Set<Dataset>()
                    .Include(x => x.Clients).ThenInclude(x => x.Loans)
                    .Single(x => x.DatasetId == datasetId);
            }
        }
    }
}
