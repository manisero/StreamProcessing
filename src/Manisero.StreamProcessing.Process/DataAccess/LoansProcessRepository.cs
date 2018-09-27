using Dapper;
using Manisero.StreamProcessing.Domain;
using Npgsql;

namespace Manisero.StreamProcessing.Process.DataAccess
{
    public interface ILoansProcessRepository
    {
        LoansProcess Create(
            LoansProcess loansProcess);
    }

    public class LoansProcessRepository : ILoansProcessRepository
    {
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
    }
}
