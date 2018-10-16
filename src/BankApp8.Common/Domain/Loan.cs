namespace BankApp8.Common.Domain
{
    public class Loan
    {
        public const int DefaultReadingBatchSize = 100000;

        public short DatasetId { get; set; }

        public int ClientId { get; set; }

        public int LoanId { get; set; }

        public decimal Value { get; set; }
    }
}
