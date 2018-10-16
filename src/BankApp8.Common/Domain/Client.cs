namespace BankApp8.Common.Domain
{
    public class Client
    {
        public const int DefaultReadingBatchSize = 100000;

        public short DatasetId { get; set; }

        public int ClientId { get; set; }
    }
}
