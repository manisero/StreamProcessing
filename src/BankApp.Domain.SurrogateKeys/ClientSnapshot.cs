using System;
using System.Collections.Generic;
using Npgsql;

namespace BankApp.Domain.SurrogateKeys
{
    public class ClientSnapshot
    {
        public static readonly Dictionary<string, Action<NpgsqlBinaryImporter, ClientSnapshot>> ColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, ClientSnapshot>>
            {
                [nameof(ClientId)] = (writer, x) => writer.Write(x.ClientId),
                [nameof(DatasetId)] = (writer, x) => writer.Write(x.DatasetId)
            };

        public long ClientSnapshotId { get; set; }

        public int ClientId { get; set; }

        public int DatasetId { get; set; }

        public Dataset Dataset { get; set; }

        public IList<LoanSnapshot> Loans { get; set; }
    }
}
