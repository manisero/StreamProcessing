﻿using System.Collections.Generic;
using BankApp.Domain.WideKeys.Data;

namespace TotalLoanCalculator2
{
    public class TotalLoanCalculationState
    {
        public ICollection<LoanSnapshot> Loans { get; set; }

        public decimal TotalLoan { get; set; }
    }
}
