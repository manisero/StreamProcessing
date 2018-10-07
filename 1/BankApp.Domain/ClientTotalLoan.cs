namespace BankApp.Domain
{
    public class ClientTotalLoan
    {
        public int ClientTotalLoanId { get; set; }

        public int ClientLoansCalculationId { get; set; }

        public long ClientSnapshotId { get; set; }

        public decimal TotalLoan { get; set; }

        public ClientLoansCalculation ClientLoansCalculation { get; set; }
    }
}
