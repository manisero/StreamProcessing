using System;
using System.Collections.Generic;
using Npgsql;

namespace BankApp.Domain.WideKeys
{
    public class ClientSnapshot
    {
        public static readonly Dictionary<string, Action<NpgsqlBinaryImporter, ClientSnapshot>> ColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, ClientSnapshot>>
            {
                [nameof(ClientId)] = (writer, x) => writer.Write(x.ClientId),
                [nameof(DatasetId)] = (writer, x) => writer.Write(x.DatasetId)
            };

        public const int DefaultReadingBatchSize = 100000;

        public short DatasetId { get; set; }

        public int ClientId { get; set; }
    }
}
