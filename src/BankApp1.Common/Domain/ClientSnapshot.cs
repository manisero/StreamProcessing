using System.Collections.Generic;

namespace BankApp1.Common.Domain
{
    public class ClientSnapshot
    {
        public long ClientSnapshotId { get; set; }

        public int ClientId { get; set; }

        public int DatasetId { get; set; }

        public Dataset Dataset { get; set; }

        public IList<LoanSnapshot> Loans { get; set; }
    }
}
