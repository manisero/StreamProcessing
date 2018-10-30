﻿using System.Collections.Generic;
using BankApp.Domain.WideKeys.Data;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp.DataAccess.Partitioned.Data
{
    public class DepositSnapshotRepository
    {
        private readonly string _connectionString;

        public DepositSnapshotRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateMany(
            IEnumerable<DepositSnapshot> items)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                PostgresCopyExecutor.ExecuteWrite(
                    connection,
                    items,
                    DepositSnapshot.ColumnMapping);
            }
        }
    }
}
