﻿using BankApp.DataAccess.WideKeys;
using BankApp.DataAccess.WideKeys.Data;
using DataProcessing.Utils;
using DataProcessing.Utils.DatabaseAccess;
using DataProcessing.Utils.Navvy;

namespace BankApp.DbSeeding.WideKeys
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = ConfigUtils.GetAppSettings(nameof(WideKeys));

            var dbCreated = DatabaseManager.TryRecreate(
                settings.ConnectionString,
                settings.DbCreationSettings.ForceRecreation,
                efContextFactory: x => new EfContext(x));

            if (!dbCreated)
            {
                return;
            }

            var taskExecutor = TaskExecutorFactory.Create();

            var taskFactory = new DbSeedingTaskFactory(
                new DatasetRepository(settings.ConnectionString),
                new ClientSnapshotRepository(settings.ConnectionString),
                new DepositSnapshotRepository(settings.ConnectionString),
                new LoanSnapshotRepository(settings.ConnectionString));

            var task = taskFactory.Create(settings.DataSettings);
            taskExecutor.Execute(task);
        }
    }
}
