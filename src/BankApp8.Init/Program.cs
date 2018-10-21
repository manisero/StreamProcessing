﻿using System;
using System.Diagnostics;
using BankApp8.Init.DbSeeding;
using DataProcessing.Utils;
using DataProcessing.Utils.DatabaseAccess;
using DataProcessing.Utils.Settings;

namespace BankApp8.Init
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigUtils.GetConfig();
            var connectionString = config.GetConnectionString();
            var dbCreationSettings = config.GetDbCreationSettings();
            var dataSettings = config.GetDataSettings();

            var dbCreated = DatabaseManager.TryRecreate(
                connectionString,
                dbCreationSettings.ForceRecreation,
                migrationScriptsAssemblySampleType: typeof(Program));

            if (!dbCreated)
            {
                return;
            }

            Console.WriteLine($"Seeding db ({dataSettings})...");
            var seedSw = Stopwatch.StartNew();
            new DbSeeder(connectionString).Seed(dataSettings);
            Console.WriteLine($"Seeding db took {seedSw.Elapsed}.");
        }
    }
}
