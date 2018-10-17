﻿using System;
using System.Collections.Generic;
using BankApp1.Common.Domain;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;
using NpgsqlTypes;

namespace BankApp1.Common.DataAccess
{
    public class LoanSnapshotRepository
    {
        private static readonly Dictionary<string, Action<NpgsqlBinaryImporter, LoanSnapshot>> ColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, LoanSnapshot>>
            {
                [nameof(LoanSnapshot.LoanId)] = (writer, x) => writer.Write(x.LoanId, NpgsqlDbType.Integer),
                [nameof(LoanSnapshot.ClientSnapshotId)] = (writer, x) => writer.Write(x.ClientSnapshotId, NpgsqlDbType.Bigint),
                [nameof(LoanSnapshot.Value)] = (writer, x) => writer.Write(x.Value, NpgsqlDbType.Numeric)
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