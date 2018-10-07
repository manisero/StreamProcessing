using System;
using System.Collections.Generic;

namespace BankApp.Domain
{
    public class Dataset
    {
        public int DatasetId { get; set; }

        public DateTime Date { get; set; }

        public IList<ClientSnapshot> Clients { get; set; }
    }
}
