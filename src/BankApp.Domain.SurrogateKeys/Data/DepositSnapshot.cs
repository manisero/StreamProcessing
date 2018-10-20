using System;
using System.Collections.Generic;
using System.Linq;
using DataProcessing.Utils;
using Npgsql;

namespace BankApp.Domain.SurrogateKeys.Data
{
    public class DepositSnapshot
    {
        public long DepositSnapshotId { get; set; }

        public int DepositId { get; set; }

        public long ClientSnapshotId { get; set; }

        public decimal Value { get; set; }

        public ClientSnapshot Client { get; set; }

        public static readonly Dictionary<string, Action<NpgsqlBinaryImporter, DepositSnapshot>> ColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, DepositSnapshot>>
            {
                [nameof(DepositId)] = (writer, x) => writer.Write(x.DepositId),
                [nameof(ClientSnapshotId)] = (writer, x) => writer.Write(x.ClientSnapshotId),
                [nameof(Value)] = (writer, x) => writer.Write(x.Value)
            };

        public static IEnumerable<DepositSnapshot> GetRandom(
            ICollection<long> clientSnapshotIds,
            int depositsPerClient)
        {
            var random = new Random();

            var depositsCount = clientSnapshotIds.Count * depositsPerClient;
            var depositIds = random
                .NextUniqueSet(depositsCount, depositsCount * 2)
                .ToArray();

            var depositsCounter = 0;

            foreach (var clientSnapshotId in clientSnapshotIds)
            {
                for (var i = 0; i < depositsPerClient; i++)
                {
                    yield return new DepositSnapshot
                    {
                        DepositId = depositIds[depositsCounter],
                        ClientSnapshotId = clientSnapshotId,
                        Value = random.Next(1000, 1000000)
                    };

                    depositsCounter++;
                }
            }
        }
    }
}
