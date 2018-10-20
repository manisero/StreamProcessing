using System;
using System.Collections.Generic;
using System.Linq;
using DataProcessing.Utils;
using Npgsql;

namespace BankApp.Domain.SurrogateKeys
{
    public class LoanSnapshot
    {
        public long LoanSnapshotId { get; set; }

        public int LoanId { get; set; }

        public long ClientSnapshotId { get; set; }

        public decimal Value { get; set; }

        public ClientSnapshot Client { get; set; }

        public static readonly Dictionary<string, Action<NpgsqlBinaryImporter, LoanSnapshot>> ColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, LoanSnapshot>>
            {
                [nameof(LoanId)] = (writer, x) => writer.Write(x.LoanId),
                [nameof(ClientSnapshotId)] = (writer, x) => writer.Write(x.ClientSnapshotId),
                [nameof(Value)] = (writer, x) => writer.Write(x.Value)
            };

        public static IEnumerable<LoanSnapshot> GetRandom(
            ICollection<long> clientSnapshotIds,
            int loansPerClient)
        {
            var random = new Random();

            var loansCount = clientSnapshotIds.Count * loansPerClient;
            var loanIds = random
                .NextUniqueSet(loansCount, loansCount * 2)
                .ToArray();

            var loansCounter = 0;

            foreach (var clientSnapshotId in clientSnapshotIds)
            {
                for (var i = 0; i < loansPerClient; i++)
                {
                    yield return new LoanSnapshot
                    {
                        LoanId = loanIds[loansCounter],
                        ClientSnapshotId = clientSnapshotId,
                        Value = random.Next(1000, 1000000)
                    };

                    loansCounter++;
                }
            }
        }
    }
}
