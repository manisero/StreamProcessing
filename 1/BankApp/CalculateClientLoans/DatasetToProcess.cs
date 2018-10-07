using System.Collections.Generic;
using BankApp.Domain;

namespace BankApp.CalculateClientLoans
{
    public class DatasetToProcess
    {
        public Dataset Dataset { get; set; }

        /// <summary>ClientId -> total loan</summary>
        public IDictionary<int, decimal> ClientLoans { get; } = new Dictionary<int, decimal>();
    }
}
