using BankApp3.Init.DbCreation;
using DataProcessing.Utils;

namespace BankApp3.Init
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigUtils.GetConfig();
            var connectionString = config.GetConnectionString();

            var dbCreated = DbCreator.TryCreate(connectionString);
        }
    }
}
