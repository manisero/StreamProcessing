﻿using System;
using System.Collections.Generic;
using BankApp3.Common.Domain;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp3.Common.DataAccess
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
