using DataProcessing.Utils.DatabaseAccess;

namespace BankApp3.Init.DbCreation
{
    public static class DbCreator
    {
        public static bool TryCreate(
            string connectionString)
        {
            return DatabaseManager.TryRecreate(
                connectionString,
                migrationScriptsAssemblySampleType: typeof(DbCreator));
        }
    }
}
