using System;
using System.Collections.Generic;
using System.Linq;
using DataProcessing.Utils;
using Npgsql;

namespace BankApp.Domain.SurrogateKeys
{
    public class ClientSnapshot
    {
        public long ClientSnapshotId { get; set; }

        public int ClientId { get; set; }

        public int DatasetId { get; set; }

        public Dataset Dataset { get; set; }

        public IList<LoanSnapshot> Loans { get; set; }

        public static readonly Dictionary<string, Action<NpgsqlBinaryImporter, ClientSnapshot>> ColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, ClientSnapshot>>
            {
                [nameof(ClientId)] = (writer, x) => writer.Write(x.ClientId),
                [nameof(DatasetId)] = (writer, x) => writer.Write(x.DatasetId)
            };

        public static IEnumerable<ClientSnapshot> GetRandom(
            int datasetId,
            int count)
        {
            var clientIds = new Random()
                .NextUniqueSet(count, count * 2)
                .ToArray();

            for (var i = 0; i < count; i++)
            {
                yield return new ClientSnapshot
                {
                    ClientId = clientIds[i],
                    DatasetId = datasetId
                };
            }
        }
    }
}
