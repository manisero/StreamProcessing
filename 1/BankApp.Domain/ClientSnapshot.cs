using System.Collections.Generic;

namespace BankApp.Domain
{
    public class ClientSnapshot
    {
        public long ClientSnapshotId { get; set; }

        public int ClientId { get; set; }

        public short DatasetId { get; set; }

        public Dataset Dataset { get; set; }

        public IList<LoanSnapshot> Loans { get; set; }
    }
}
