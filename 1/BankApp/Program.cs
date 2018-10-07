using BankApp.DataAccess;
using BankApp.Utils;

namespace BankApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigUtils.GetConfig();
            var connectionString = config.GetDefaultConnectionString();

            var efContext = new EfContext(connectionString);
            efContext.Database.EnsureCreated();
        }
    }
}
