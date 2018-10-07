using System;
using System.Collections.Generic;
using BankApp.Domain;
using BankApp.Utils.DatabaseAccess;
using Npgsql;
using NpgsqlTypes;

namespace BankApp.DataAccess
{
    public class DatasetRepository
    {
        private static readonly Dictionary<string, Action<NpgsqlBinaryImporter, Dataset>> ColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, Dataset>>
            {
                [nameof(Dataset.DatasetId)] = (writer, x) => writer.Write(x.DatasetId, NpgsqlDbType.Smallint),
                [nameof(Dataset.Date)] = (writer, x) => writer.Write(x.Date, NpgsqlDbType.Date)
            };

        private readonly string _connectionString;

        public DatasetRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateMany(
            IEnumerable<Dataset> items)
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
