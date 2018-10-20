using System.Collections.Generic;
using BankApp.Domain.WideKeys.Data;

namespace BankApp3.Main.MaxLossCalculationTask
{
    public class MaxLossCalculationState
    {
        public ICollection<DepositSnapshot> Deposits { get; set; }

        public ICollection<LoanSnapshot> Loans { get; set; }

        public decimal MaxLoss { get; set; }
    }
}
