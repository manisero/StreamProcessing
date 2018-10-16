using System.Collections.Generic;
using BankApp8.Common.Domain;

namespace BankApp8.Main.ClientLoansCalculationTask
{
    public class ClientsToProcess
    {
        /// <summary>ClientId -> ClientToProcess</summary>
        public IDictionary<int, ClientToProcess> Clients { get; set; }
    }

    public class ClientToProcess
    {
        public Client Client { get; set; }

        public ICollection<Loan> Loans { get; set; }

        public decimal TotalLoan { get; set; }
    }
}
