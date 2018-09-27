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
            const string sql = @"
insert into ""LoansProcess""
(""DatasetId"") values
(@DatasetId)
returning *";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return connection.QuerySingle<LoansProcess>(sql, loansProcess);
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
