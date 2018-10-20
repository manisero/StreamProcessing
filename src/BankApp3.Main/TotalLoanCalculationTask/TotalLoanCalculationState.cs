using System.Collections.Generic;
using BankApp.Domain.WideKeys;
using BankApp.Domain.WideKeys.Data;

namespace BankApp3.Main.TotalLoanCalculationTask
{
    public class TotalLoanCalculationState
    {
        public ICollection<LoanSnapshot> Loans { get; set; }

        public decimal TotalLoan { get; set; }
    }
}
