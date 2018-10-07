namespace BankApp.Domain
{
    public class ClientTotalLoan
    {
        public long ClientTotalLoanId { get; set; }

        public int ClientLoansCalculationId { get; set; }

        public int ClientId { get; set; }

        public decimal TotalLoan { get; set; }

        public ClientLoansCalculation ClientLoansCalculation { get; set; }
    }
}
