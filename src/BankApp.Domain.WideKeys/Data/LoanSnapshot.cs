using System;
using System.Collections.Generic;
using System.Linq;
using DataProcessing.Utils;
using Npgsql;

namespace BankApp.Domain.WideKeys.Data
{
    public class LoanSnapshot
    {
        public short DatasetId { get; set; }

        public int ClientId { get; set; }

        public int LoanId { get; set; }

        public decimal Value { get; set; }

        public ClientSnapshot ClientSnapshot { get; set; }

        public const int DefaultReadingBatchSize = 100000;

        public static readonly Dictionary<string, Action<NpgsqlBinaryImporter, LoanSnapshot>> ColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, LoanSnapshot>>
            {
                [nameof(DatasetId)] = (writer, x) => writer.Write(x.DatasetId),
                [nameof(ClientId)] = (writer, x) => writer.Write(x.ClientId),
                [nameof(LoanId)] = (writer, x) => writer.Write(x.LoanId),
                [nameof(Value)] = (writer, x) => writer.Write(x.Value)
            };

        public static readonly Func<NpgsqlBinaryExporter, LoanSnapshot> RowReader =
            x => new LoanSnapshot
            {
                DatasetId = x.Read<short>(),
                ClientId = x.Read<int>(),
                LoanId = x.Read<int>(),
                Value = x.Read<decimal>()
            };

        public static IEnumerable<LoanSnapshot> GetRandom(
            short datasetId,
            ICollection<int> clientIds,
            int loansPerClient)
        {
            var random = new Random();

            var loansCount = clientIds.Count * loansPerClient;
            var loanIds = random
                .NextUniqueSet(loansCount, loansCount * 2)
                .OrderBy(x => x)
                .ToArray();

            var loansCounter = 0;

            foreach (var clientId in clientIds)
            {
                for (var i = 0; i < loansPerClient; i++)
                {
                    yield return new LoanSnapshot
                    {
                        DatasetId = datasetId,
                        ClientId = clientId,
                        LoanId = loanIds[loansCounter],
                        Value = random.Next(1000, 1000000)
                    };

                    loansCounter++;
                }
            }
        }
    }
}
