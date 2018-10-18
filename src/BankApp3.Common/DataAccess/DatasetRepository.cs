using System;
using System.Collections.Generic;
using BankApp3.Common.Domain;
using Dapper;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;
using NpgsqlTypes;

namespace BankApp3.Common.DataAccess
{
    public class DatasetRepository
    {
        private static readonly Dictionary<string, Action<NpgsqlBinaryImporter, Dataset>> ColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, Dataset>>
            {
                [nameof(Dataset.Date)] = (writer, x) => writer.Write(x.Date, NpgsqlDbType.Date)
            };

        private readonly string _connectionString;

        public DatasetRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public ICollection<Dataset> GetAll()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return connection
                    .Query<Dataset>($@"SELECT * FROM ""{nameof(Dataset)}""")
                    .AsList();
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
                    ColumnMapping);
            }
        }
    }
}
