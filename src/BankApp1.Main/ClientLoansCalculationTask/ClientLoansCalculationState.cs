using System.Collections.Generic;
using BankApp.Domain.SurrogateKeys.Data;

namespace BankApp1.Main.ClientLoansCalculationTask
{
    public class ClientLoansCalculationState
    {
        public Dataset Dataset { get; set; }

        /// <summary>ClientId -> total loan</summary>
        public IDictionary<int, decimal> ClientLoans { get; } = new Dictionary<int, decimal>();
    }
}
