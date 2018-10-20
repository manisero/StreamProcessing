using System;
using System.Collections.Generic;
using Npgsql;
using NpgsqlTypes;

namespace BankApp1.Common.Domain
{
    public class Dataset
    {
        public static readonly Dictionary<string, Action<NpgsqlBinaryImporter, Dataset>> ColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, Dataset>>
            {
                [nameof(Date)] = (writer, x) => writer.Write(x.Date, NpgsqlDbType.Date)
            };

        public int DatasetId { get; set; }

        public DateTime Date { get; set; }

        public IList<ClientSnapshot> Clients { get; set; }
    }
}
