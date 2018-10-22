using System.Collections.Generic;
using BankApp.Domain.WideKeys.Data;

namespace ClientLoanCalculator1
{
    public class ClientLoansCalculationState
    {
        public ICollection<LoanSnapshot> Loans { get; set; }

        /// <summary>ClientId -> total loan</summary>
        public IDictionary<int, decimal> ClientLoans { get; } = new Dictionary<int, decimal>();
    }
}
