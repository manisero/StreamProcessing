namespace BankApp3.Common.Domain
{
    public class LoanSnapshot
    {
        public short DatasetId { get; set; }

        public int ClientId { get; set; }

        public int LoanId { get; set; }

        public decimal Value { get; set; }
    }
}
