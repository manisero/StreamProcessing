using BankApp.Domain.SurrogateKeys.Data;

namespace BankApp.Domain.SurrogateKeys.Tasks
{
    public class TotalLoanCalculation
    {
        public int TotalLoanCalculationId { get; set; }

        public int DatasetId { get; set; }

        public decimal? TotalLoan { get; set; }

        public Dataset Dataset { get; set; }
    }
}
