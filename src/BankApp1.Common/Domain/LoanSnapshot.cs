namespace BankApp1.Common.Domain
{
    public class LoanSnapshot
    {
        public long LoanSnapshotId { get; set; }

        public int LoanId { get; set; }

        public long ClientSnapshotId { get; set; }

        public decimal Value { get; set; }

        public ClientSnapshot Client { get; set; }
    }
}
