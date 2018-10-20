using System;
using System.Collections.Generic;
using Npgsql;
using NpgsqlTypes;

namespace BankApp.Domain.SurrogateKeys
{
    public class Dataset
    {
        public int DatasetId { get; set; }

        public DateTime Date { get; set; }

        public IList<ClientSnapshot> Clients { get; set; }

        public static readonly Dictionary<string, Action<NpgsqlBinaryImporter, Dataset>> ColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, Dataset>>
            {
                [nameof(Date)] = (writer, x) => writer.Write(x.Date, NpgsqlDbType.Date)
            };

        public static IEnumerable<Dataset> GetRandom(
            int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new Dataset
                {
                    Date = new DateTime(2018, 1, 1).AddMonths(i)
                };
            }
        }
    }
}
