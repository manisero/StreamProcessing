using System.Collections.Generic;

namespace BankApp.Domain.SurrogateKeys
{
    public class ClientLoansCalculation
    {
        public int ClientLoansCalculationId { get; set; }

        public int DatasetId { get; set; }

        public Dataset Dataset { get; set; }

        public ICollection<ClientTotalLoan> ClientTotalLoans { get; set; }
    }
}
