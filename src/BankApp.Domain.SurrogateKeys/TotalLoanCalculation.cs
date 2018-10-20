namespace BankApp.Domain.SurrogateKeys
{
    public class TotalLoanCalculation
    {
        public int TotalLoanCalculationId { get; set; }

        public int DatasetId { get; set; }

        public decimal? TotalLoan { get; set; }

        public Dataset Dataset { get; set; }
    }
}
