using System.Collections.Generic;

namespace BankApp3.Init.DbSeeding
{
    public class DatasetToPopulate
    {
        public short DatasetId { get; set; }

        public ICollection<int> ClientIds { get; set; }
    }
}
