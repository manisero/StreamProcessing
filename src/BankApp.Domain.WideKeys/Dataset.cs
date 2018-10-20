using System;
using System.Collections.Generic;
using Npgsql;
using NpgsqlTypes;

namespace BankApp.Domain.WideKeys
{
    public class Dataset
    {
        public static readonly Dictionary<string, Action<NpgsqlBinaryImporter, Dataset>> ColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, Dataset>>
            {
                [nameof(Date)] = (writer, x) => writer.Write(x.Date, NpgsqlDbType.Date)
            };

        public short DatasetId { get; set; }

        public DateTime Date { get; set; }
    }
}
