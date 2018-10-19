﻿using System;
using System.Diagnostics;
using BankApp1.Common.DataAccess;
using BankApp1.Init.DbSeeding;
using DataProcessing.Utils;
using DataProcessing.Utils.DatabaseAccess;
using DataProcessing.Utils.DataSeeding;

namespace BankApp1.Init
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigUtils.GetConfig();
            var connectionString = config.GetConnectionString();
            var dataSetup = config.GetDataSetup();

            var dbCreated = DatabaseManager.TryRecreate(
                connectionString,
                efContextFactory: x => new EfContext(x));

            if (!dbCreated)
            {
                return;
            }

            Console.WriteLine($"Seeding db ({dataSetup})...");
            var seedSw = Stopwatch.StartNew();
            new DbSeeder(connectionString).Seed(dataSetup);
            Console.WriteLine($"Seeding db took {seedSw.Elapsed}.");
        }
    }
}
