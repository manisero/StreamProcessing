﻿using BankApp.Domain.WideKeys.Data;

namespace BankApp.Domain.WideKeys.Tasks
{
    public class TotalLoanCalculation
    {
        public short TotalLoanCalculationId { get; set; }

        public short DatasetId { get; set; }

        public decimal? TotalLoan { get; set; }

        public Dataset Dataset { get; set; }
    }
}
