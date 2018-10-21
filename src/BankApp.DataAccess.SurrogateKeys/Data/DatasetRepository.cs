﻿using System.Collections.Generic;
using System.Linq;
using BankApp.Domain.SurrogateKeys.Data;
using DataProcessing.Utils.DatabaseAccess;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BankApp.DataAccess.SurrogateKeys.Data
{
    public class DatasetRepository
    {
        private readonly string _connectionString;

        public DatasetRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public ICollection<Dataset> GetAll()
        {
            using (var context = new EfContext(_connectionString))
            {
                return context.Set<Dataset>().ToList();
            }
        }

        public Dataset Get(
            int datasetId)
        {
            using (var context = new EfContext(_connectionString))
            {
                return context
                    .Set<Dataset>()
                    .Include(x => x.Clients).ThenInclude(x => x.Deposits)
                    .Include(x => x.Clients).ThenInclude(x => x.Loans)
                    .Single(x => x.DatasetId == datasetId);
            }
        }

        public int? GetMaxId()
        {
            using (var context = new EfContext(_connectionString))
            {
                return context.Set<Dataset>().Select(x => x.DatasetId).Max();
            }
        }

        public void CreateMany(
            IEnumerable<Dataset> items)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                PostgresCopyExecutor.Execute(
                    connection,
                    items,
                    Dataset.ColumnMapping);
            }
        }
    }
}