using System.Collections.Generic;
using BankApp.Domain.WideKeys;
using Dapper;
using DataProcessing.Utils.DatabaseAccess;
using Npgsql;

namespace BankApp8.Common.DataAccess
{
    public class ClientLoansCalculationRepository
    {
        private readonly string _connectionString;

        public ClientLoansCalculationRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public ClientLoansCalculation Create(
            ClientLoansCalculation item)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var loansProcessSql = $@"
INSERT INTO ""{nameof(ClientLoansCalculation)}""
(""{nameof(ClientLoansCalculation.DatasetId)}"") values
(@DatasetId)
RETURNING *";

                var calculation = connection.QuerySingle<ClientLoansCalculation>(loansProcessSql, item);

                var partitionSql = $@"
CREATE TABLE ""{nameof(ClientTotalLoan)}_{calculation.ClientLoansCalculationId}""
PARTITION OF ""{nameof(ClientTotalLoan)}""
(CONSTRAINT ""PK_{nameof(ClientTotalLoan)}_{calculation.ClientLoansCalculationId}"" PRIMARY KEY (""{nameof(ClientTotalLoan.ClientLoansCalculationId)}"", ""{nameof(ClientTotalLoan.ClientId)}""))
FOR VALUES IN ({calculation.ClientLoansCalculationId})";

                connection.Execute(partitionSql);
                
                return calculation;
            }
        }

        public void SaveClientTotalLoans(
            IEnumerable<ClientTotalLoan> items)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                PostgresCopyExecutor.Execute(
                    connection,
                    items,
                    ClientTotalLoan.ColumnMapping);
            }
        }
    }
}
