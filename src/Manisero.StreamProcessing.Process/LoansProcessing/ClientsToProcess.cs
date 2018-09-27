using System.Collections.Generic;
using Manisero.StreamProcessing.Domain;

namespace Manisero.StreamProcessing.Process.LoansProcessing
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
