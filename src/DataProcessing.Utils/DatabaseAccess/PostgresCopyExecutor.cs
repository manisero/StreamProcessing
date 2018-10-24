using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;

namespace DataProcessing.Utils.DatabaseAccess
{
    public static class PostgresCopyExecutor
    {
        public static void ExecuteWrite<TRow>(
            NpgsqlConnection connection,
            IEnumerable<TRow> rows,
            IDictionary<string, Action<NpgsqlBinaryImporter, TRow>> columnMapping)
        {
            var columnsString = columnMapping.Keys.Select(x => $"\"{x}\"").JoinWithSeparator(", ");
            var command = $"COPY \"{typeof(TRow).Name}\" ({columnsString}) FROM STDIN BINARY;";

            using (var writer = connection.OpenIfClosed().BeginBinaryImport(command))
            {
                foreach (var row in rows)
                {
                    writer.StartRow();

                    foreach (var column in columnMapping)
                    {
                        column.Value(writer, row);
                    }
                }

                writer.Complete();
            }
        }

        public static ICollection<TRow> ExecuteRead<TRow>(
            NpgsqlConnection connection,
            Func<NpgsqlBinaryExporter, TRow> rowReader)
        {
            var columnsString = typeof(TRow)
                .GetProperties()
                .Where(x => x.PropertyType.Namespace.StartsWith("System"))
                .Select(x => $"\"{x.Name}\"")
                .JoinWithSeparator(", ");

            var command = $"COPY \"{typeof(TRow).Name}\" ({columnsString}) TO STDOUT BINARY;";

            using (var reader = connection.OpenIfClosed().BeginBinaryExport(command))
            {
                var rows = new List<TRow>();

                while (reader.StartRow() != -1)
                {
                    var row = rowReader(reader);
                    rows.Add(row);
                }

                return rows;
            }
        }
    }
}
