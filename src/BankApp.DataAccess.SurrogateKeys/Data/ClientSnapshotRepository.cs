﻿using System.Collections.Generic;
using System.Linq;
using BankApp.Domain.SurrogateKeys;
using BankApp.Domain.SurrogateKeys.Data;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp.DataAccess.SurrogateKeys.Data
{
    public class ClientSnapshotRepository
    {
        private readonly string _connectionString;

        public ClientSnapshotRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public ICollection<ClientSnapshot> GetForDataset(
            int datasetId)
        {
            using (var context = new EfContext(_connectionString))
            {
                return context.Set<ClientSnapshot>().Where(x => x.DatasetId == datasetId).ToList();
            }
        }

        public void CreateMany(
            IEnumerable<ClientSnapshot> items)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                PostgresCopyExecutor.ExecuteWrite(
                    connection,
                    items,
                    ClientSnapshot.ColumnMapping);
            }
        }
    }
}
