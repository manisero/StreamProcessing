namespace BankApp.DataAccess.Partitioned.Data
{
    public class DepositSnapshotRepository : WideKeys.Data.DepositSnapshotRepository
    {
        public DepositSnapshotRepository(
            string connectionString)
            : base(connectionString)
        {
        }
    }
}
