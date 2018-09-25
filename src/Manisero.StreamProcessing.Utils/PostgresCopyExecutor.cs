using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;

namespace Manisero.StreamProcessing.Utils
{
    public static class PostgresCopyExecutor
    {
        public static void Execute<TRow>(
            NpgsqlConnection connection,
            IEnumerable<TRow> rows,
            IDictionary<string, Action<NpgsqlBinaryImporter, TRow>> columnMapping)
        {
            var copyCommand = GetCopyCommand(typeof(TRow).Name, columnMapping.Keys);
            
            using (var writer = connection.OpenIfClosed().BeginBinaryImport(copyCommand))
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

        private static string GetCopyCommand(
            string tableName,
            ICollection<string> columnNames)
        {
            var columnsString = string.Join(", ", columnNames.Select(x => $"\"{x}\""));

            return $"COPY \"{tableName}\" ({columnsString}) FROM STDIN BINARY;";
        }
    }
}
