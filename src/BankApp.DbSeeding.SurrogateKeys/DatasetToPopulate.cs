using System.Collections.Generic;

namespace BankApp.DbSeeding.SurrogateKeys
{
    public class DatasetToPopulate
    {
        public int DatasetId { get; set; }

        public ICollection<long> ClientSnapshotIds { get; set; }
    }
}
