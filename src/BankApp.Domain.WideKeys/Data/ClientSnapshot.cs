using System;
using System.Collections.Generic;
using System.Linq;
using DataProcessing.Utils;
using Npgsql;

namespace BankApp.Domain.WideKeys.Data
{
    public class ClientSnapshot
    {
        public short DatasetId { get; set; }

        public int ClientId { get; set; }

        public Dataset Dataset { get; set; }

        public const int DefaultReadingBatchSize = 100000;

        public static readonly Dictionary<string, Action<NpgsqlBinaryImporter, ClientSnapshot>> ColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, ClientSnapshot>>
            {
                [nameof(ClientId)] = (writer, x) => writer.Write(x.ClientId),
                [nameof(DatasetId)] = (writer, x) => writer.Write(x.DatasetId)
            };

        public static IEnumerable<ClientSnapshot> GetRandom(
            short datasetId,
            int count)
        {
            var clientIds = new Random()
                .NextUniqueSet(count, count * 2)
                .OrderBy(x => x)
                .ToArray();

            for (var i = 0; i < count; i++)
            {
                yield return new ClientSnapshot
                {
                    DatasetId = datasetId,
                    ClientId = clientIds[i]
                };
            }
        }
    }
}
