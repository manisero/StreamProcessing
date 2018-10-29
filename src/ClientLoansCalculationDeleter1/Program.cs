using BankApp.DataAccess.WideKeys.Tasks;
using DataProcessing.Utils;

namespace ClientLoansCalculationDeleter1
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = ConfigUtils.GetAppSettings();

            var clientLoansCalculationRepository = new ClientLoansCalculationRepository(settings.ConnectionString);
        }
    }
}
