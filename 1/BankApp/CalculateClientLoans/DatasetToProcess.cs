using System.Collections.Generic;
using BankApp.Domain;

namespace BankApp.CalculateClientLoans
{
    public class DatasetToProcess
    {
        public Dataset Dataset { get; set; }

        /// <summary>ClientSnapshotId -> total loan</summary>
        public IDictionary<long, decimal> ClientLoans { get; } = new Dictionary<long, decimal>();
    }
}
