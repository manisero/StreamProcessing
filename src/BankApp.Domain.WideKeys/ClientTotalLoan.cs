namespace BankApp.Domain.WideKeys
{
    public class ClientTotalLoan
    {
        public short ClientLoansCalculationId { get; set; }

        public int ClientId { get; set; }

        public decimal TotalLoan { get; set; }
    }
}
