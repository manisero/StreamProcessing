using System.Collections.Generic;
using Manisero.StreamProcessing.Domain;

namespace Manisero.StreamProcessing.Process.LoansProcessing
{
    public class ClientsToProcess
    {
        public ICollection<Client> Clients { get; set; }
        
        /// <summary>ClientId -> Loans</summary>
        public IDictionary<int, ICollection<Loan>> Loans { get; set; }
    }
}
