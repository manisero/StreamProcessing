namespace BankApp.DataAccess.Partitioned.Tasks
{
    public class ClientTotalLoanRepository : WideKeys.Tasks.ClientTotalLoanRepository
    {
        public ClientTotalLoanRepository(
            string connectionString)
            : base(connectionString)
        {
        }
    }
}
