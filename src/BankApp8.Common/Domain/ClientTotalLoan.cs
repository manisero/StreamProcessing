namespace BankApp8.Common.Domain
{
    public class ClientTotalLoan
    {
        public short ClientLoansCalculationId { get; set; }

        public int ClientId { get; set; }

        public decimal TotalLoan { get; set; }
    }
}
