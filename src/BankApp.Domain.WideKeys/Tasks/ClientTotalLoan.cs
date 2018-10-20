using System;
using System.Collections.Generic;
using Npgsql;

namespace BankApp.Domain.WideKeys.Tasks
{
    public class ClientTotalLoan
    {
        public short ClientLoansCalculationId { get; set; }

        public int ClientId { get; set; }

        public decimal TotalLoan { get; set; }

        public static readonly Dictionary<string, Action<NpgsqlBinaryImporter, ClientTotalLoan>> ColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, ClientTotalLoan>>
            {
                [nameof(ClientLoansCalculationId)] = (writer, x) => writer.Write(x.ClientLoansCalculationId),
                [nameof(ClientId)] = (writer, x) => writer.Write(x.ClientId),
                [nameof(TotalLoan)] = (writer, x) => writer.Write(x.TotalLoan)
            };
    }
}
