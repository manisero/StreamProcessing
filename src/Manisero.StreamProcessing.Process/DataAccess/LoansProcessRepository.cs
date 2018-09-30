using System;
using System.Collections.Generic;
using Dapper;
using Manisero.StreamProcessing.Domain;
using Manisero.StreamProcessing.Utils.DataAccess;
using Npgsql;
using NpgsqlTypes;

namespace Manisero.StreamProcessing.Process.DataAccess
{
    public interface ILoansProcessRepository
    {
        LoansProcess Create(
            LoansProcess loansProcess);

        void SaveClientResults(
            IEnumerable<LoansProcessClientResult> results);
    }

    public class LoansProcessRepository : ILoansProcessRepository
    {
        private static readonly Dictionary<string, Action<NpgsqlBinaryImporter, LoansProcessClientResult>> LoansProcessClientResultColumnMapping =
            new Dictionary<string, Action<NpgsqlBinaryImporter, LoansProcessClientResult>>
            {
                [nameof(LoansProcessClientResult.LoansProcessId)] = (writer, x) => writer.Write(x.LoansProcessId, NpgsqlDbType.Smallint),
                [nameof(LoansProcessClientResult.ClientId)] = (writer, x) => writer.Write(x.ClientId, NpgsqlDbType.Integer),
                [nameof(LoansProcessClientResult.TotalLoan)] = (writer, x) => writer.Write(x.TotalLoan, NpgsqlDbType.Numeric)
            };

        private readonly string _connectionString;

        public LoansProcessRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public LoansProcess Create(
            LoansProcess loansProcess)
        {
            const string loansProcessSql = @"
insert into ""LoansProcess""
(""DatasetId"") values
(@DatasetId)
returning *";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var process = connection.QuerySingle<LoansProcess>(loansProcessSql, loansProcess);

                var partitionsSql = $@"
CREATE TABLE ""LoansProcessClientResult_{process.LoansProcessId}""
PARTITION OF ""LoansProcessClientResult""
(CONSTRAINT ""PK_LoansProcessClientResult_{process.LoansProcessId}"" PRIMARY KEY (""LoansProcessId"", ""ClientId""))
FOR VALUES IN ({process.LoansProcessId})";

                connection.Execute(partitionsSql);
                
                return process;
            }
        }

        public void SaveClientResults(
            IEnumerable<LoansProcessClientResult> results)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                PostgresCopyExecutor.Execute(
                    connection,
                    results,
                    LoansProcessClientResultColumnMapping);
            }
        }
    }
}
