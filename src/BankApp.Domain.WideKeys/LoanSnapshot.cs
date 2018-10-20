using System;
using System.Collections.Generic;
using Npgsql;

namespace BankApp.Domain.WideKeys
{
    public class LoanSnapshot
    {
        public static readonly Dictionary<string, Action<NpgsqlBinaryImporter, LoanSnapshot>> ColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, LoanSnapshot>>
            {
                [nameof(DatasetId)] = (writer, x) => writer.Write(x.DatasetId),
                [nameof(ClientId)] = (writer, x) => writer.Write(x.ClientId),
                [nameof(LoanId)] = (writer, x) => writer.Write(x.LoanId),
                [nameof(Value)] = (writer, x) => writer.Write(x.Value)
            };

        public const int DefaultReadingBatchSize = 100000;

        public short DatasetId { get; set; }

        public int ClientId { get; set; }

        public int LoanId { get; set; }

        public decimal Value { get; set; }
    }
}
