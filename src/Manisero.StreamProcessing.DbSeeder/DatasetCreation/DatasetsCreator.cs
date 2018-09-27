using System;
using System.Collections.Generic;
using Manisero.StreamProcessing.Domain;
using Manisero.StreamProcessing.Utils;
using Manisero.StreamProcessing.Utils.DataAccess;
using Npgsql;
using NpgsqlTypes;

namespace Manisero.StreamProcessing.DbSeeder.DatasetCreation
{
    public static class DatasetsCreator
    {
        private static readonly Dictionary<string, Action<NpgsqlBinaryImporter, Dataset>> ColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, Dataset>>
            {
                [nameof(Dataset.DatasetId)] = (writer, x) => writer.Write(x.DatasetId, NpgsqlDbType.Smallint),
                [nameof(Dataset.Date)] = (writer, x) => writer.Write(x.Date, NpgsqlDbType.Date)
            };

        public static IDictionary<short, Dataset> Create(
            string connectionString)
        {
            var datasets = new Dictionary<short, Dataset>();

            for (short datasetId = 1; datasetId <= 9; datasetId++)
            {
                datasets.Add(
                    datasetId,
                    new Dataset
                    {
                        DatasetId = datasetId,
                        Date = new DateTime(2018, 1, 1).AddMonths(datasetId - 1)
                    });
            }

            using (var connection = new NpgsqlConnection(connectionString))
            {
                PostgresCopyExecutor.Execute(
                    connection,
                    datasets.Values,
                    ColumnMapping);
            }

            return datasets;
        }
    }
}
