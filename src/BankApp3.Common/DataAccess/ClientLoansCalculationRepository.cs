﻿using BankApp3.Common.Domain;

namespace BankApp3.Common.DataAccess
{
    public class ClientLoansCalculationRepository
    {
        private readonly string _connectionString;

        public ClientLoansCalculationRepository(
            string connectionString)
        {
            _connectionString = connectionString;
        }

        public ClientLoansCalculation Create(
            ClientLoansCalculation item)
        {
            using (var context = new EfContext(_connectionString))
            {
                context.Set<ClientLoansCalculation>().Add(item);
                context.SaveChanges();

                return item;
            }
        }
    }
}
