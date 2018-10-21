using System;
using System.Collections.Generic;
using System.Linq;
using DataProcessing.Utils;
using Npgsql;

namespace BankApp.Domain.WideKeys.Data
{
    public class DepositSnapshot
    {
        public short DatasetId { get; set; }

        public int ClientId { get; set; }

        public int DepositId { get; set; }

        public decimal Value { get; set; }

        public ClientSnapshot ClientSnapshot { get; set; }

        public const int DefaultReadingBatchSize = 100000;

        public static readonly Dictionary<string, Action<NpgsqlBinaryImporter, DepositSnapshot>> ColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, DepositSnapshot>>
            {
                [nameof(DatasetId)] = (writer, x) => writer.Write(x.DatasetId),
                [nameof(ClientId)] = (writer, x) => writer.Write(x.ClientId),
                [nameof(DepositId)] = (writer, x) => writer.Write(x.DepositId),
                [nameof(Value)] = (writer, x) => writer.Write(x.Value)
            };

        public static IEnumerable<DepositSnapshot> GetRandom(
            short datasetId,
            ICollection<int> clientIds,
            int depositsPerClient)
        {
            var random = new Random();

            var depositsCount = clientIds.Count * depositsPerClient;
            var depositIds = random
                .NextUniqueSet(depositsCount, depositsCount * 2)
                .ToArray();

            var depositsCounter = 0;

            foreach (var clientId in clientIds)
            {
                for (var i = 0; i < depositsPerClient; i++)
                {
                    yield return new DepositSnapshot
                    {
                        DatasetId = datasetId,
                        ClientId = clientId,
                        DepositId = depositIds[depositsCounter],
                        Value = random.Next(1000, 1000000)
                    };

                    depositsCounter++;
                }
            }
        }
    }
}
