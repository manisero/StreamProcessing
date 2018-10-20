using System.Collections.Generic;
using BankApp.Domain.WideKeys;

namespace BankApp8.Main.ClientLoansCalculationTask
{
    public class ClientsToProcess
    {
        /// <summary>ClientId -> ClientToProcess</summary>
        public IDictionary<int, ClientToProcess> Clients { get; set; }
    }

    public class ClientToProcess
    {
        public ClientSnapshot Client { get; set; }

        public ICollection<LoanSnapshot> Loans { get; set; }

        public decimal TotalLoan { get; set; }
    }
}
