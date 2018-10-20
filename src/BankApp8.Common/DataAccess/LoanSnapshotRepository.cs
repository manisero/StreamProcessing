﻿using System;
using System.Collections.Generic;
using BankApp8.Common.Domain;
using Dapper;
using DataProcessing.Utils;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp8.Common.DataAccess
{
    public class LoanSnapshotRepository
    {
        private static readonly Dictionary<string, Action<NpgsqlBinaryImporter, LoanSnapshot>> ColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, LoanSnapshot>>
            {
                [nameof(LoanSnapshot.DatasetId)] = (writer, x) => writer.Write(x.DatasetId),
                [nameof(LoanSnapshot.ClientId)] = (writer, x) => writer.Write(x.ClientId),
                [nameof(LoanSnapshot.LoanId)] = (writer, x) => writer.Write(x.LoanId),
                [nameof(LoanSnapshot.Value)] = (writer, x) => writer.Write(x.Value)
            };

        private readonly string _connectionString;

        public LoanSnapshotRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>Returns ClientId -> Loans</summary>
        public IDictionary<int, ICollection<LoanSnapshot>> GetRange(
            short datasetId,
            int firstClientId,
            int lastClientId)
        {
            var sql = $@"
select *
from ""{nameof(LoanSnapshot)}""
where
  ""{nameof(LoanSnapshot.DatasetId)}"" = @DatasetId and
  ""{nameof(LoanSnapshot.ClientId)}"" between @FirstClientId and @LastClientId";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return connection
                    .Query<LoanSnapshot>(
                        sql,
                        new
                        {
                            DatasetId = datasetId,
                            FirstClientId = firstClientId,
                            LastClientId = lastClientId
                        },
                        buffered: false)
                    .GroupAndDict(
                        x => x.ClientId,
                        x => x.ToICollection());
            }
        }

        public void CreateMany(
            IEnumerable<LoanSnapshot> items)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                PostgresCopyExecutor.Execute(
                    connection,
                    items,
                    ColumnMapping);
            }
        }
    }
}