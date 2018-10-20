using System.Collections.Generic;
using BankApp.Domain.SurrogateKeys.Data;

namespace BankApp.Domain.SurrogateKeys.Tasks
{
    public class ClientLoansCalculation
    {
        public int ClientLoansCalculationId { get; set; }

        public int DatasetId { get; set; }

        public Dataset Dataset { get; set; }

        public ICollection<ClientTotalLoan> ClientTotalLoans { get; set; }
    }
}
