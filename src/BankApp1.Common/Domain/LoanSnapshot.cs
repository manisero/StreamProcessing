using System;
using System.Collections.Generic;
using Npgsql;

namespace BankApp1.Common.Domain
{
    public class LoanSnapshot
    {
        public static readonly Dictionary<string, Action<NpgsqlBinaryImporter, LoanSnapshot>> ColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, LoanSnapshot>>
            {
                [nameof(LoanId)] = (writer, x) => writer.Write(x.LoanId),
                [nameof(ClientSnapshotId)] = (writer, x) => writer.Write(x.ClientSnapshotId),
                [nameof(Value)] = (writer, x) => writer.Write(x.Value)
            };

        public long LoanSnapshotId { get; set; }

        public int LoanId { get; set; }

        public long ClientSnapshotId { get; set; }

        public decimal Value { get; set; }

        public ClientSnapshot Client { get; set; }
    }
}
