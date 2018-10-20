using BankApp.Domain.SurrogateKeys.Data;

namespace BankApp1.Main.MaxLossCalculationTask
{
    public class MaxLossCalculationState
    {
        public Dataset Dataset { get; set; }

        public decimal MaxLoss { get; set; }
    }
}
