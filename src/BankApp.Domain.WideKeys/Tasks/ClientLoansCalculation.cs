﻿using BankApp.Domain.WideKeys.Data;

namespace BankApp.Domain.WideKeys.Tasks
{
    public class ClientLoansCalculation
    {
        public short ClientLoansCalculationId { get; set; }

        public short DatasetId { get; set; }

        public Dataset Dataset { get; set; }
    }
}
