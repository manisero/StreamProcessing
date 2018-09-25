using System;
using System.Collections.Generic;
using System.Linq;
using Manisero.StreamProcessing.Domain;
using Manisero.StreamProcessing.Utils;
using Npgsql;
using NpgsqlTypes;

namespace Manisero.StreamProcessing.DbSeeder.DatasetCreation
{
    public static class DatasetsCreator
    {
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

            using (var connection = new NpgsqlConnection(connectionString).OpenIfClosed())
            using (var writer = connection.BeginBinaryImport(GetCopyCommand<Dataset>()))
            {
                foreach (var set in datasets.Values)
                {
                    writer.StartRow();
                    writer.Write(set.DatasetId, NpgsqlDbType.Smallint);
                    writer.Write(set.Date, NpgsqlDbType.Date);
                }

                writer.Complete();
            }

            return datasets;
        }

        private static string GetCopyCommand<TEntity>()
        {
            var entityType = typeof(TEntity);

            var columns = entityType.GetProperties().Select(x => x.Name);
            var columnsString = string.Join(", ", columns.Select(x => $"\"{x}\""));

            return $"COPY \"{entityType.Name}\" ({columnsString}) FROM STDIN BINARY;";
        }
    }
}
