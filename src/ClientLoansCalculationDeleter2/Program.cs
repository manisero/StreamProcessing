using System;
using System.Diagnostics;
using BankApp.DataAccess.Partitioned.Tasks;
using DataProcessing.Utils;
using DataProcessing.Utils.DatabaseAccess;

namespace ClientLoansCalculationDeleter2
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = ConfigUtils.GetAppSettings();

            var clientLoansCalculationRepository = new ClientLoansCalculationRepository(
                settings.ConnectionString,
                new DatabaseManager(settings.ConnectionString));

            var id = clientLoansCalculationRepository.GetMaxId();

            var sw = Stopwatch.StartNew();
            clientLoansCalculationRepository.Delete(id.Value);
            sw.Stop();

            Console.WriteLine($"Took {sw.Elapsed}.");
        }
    }
}
