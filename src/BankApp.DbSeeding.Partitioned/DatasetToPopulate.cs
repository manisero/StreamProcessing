using System.Collections.Generic;

namespace BankApp.DbSeeding.Partitioned
{
    public class DatasetToPopulate
    {
        public short DatasetId { get; set; }

        public ICollection<int> ClientIds { get; set; }
    }
}
