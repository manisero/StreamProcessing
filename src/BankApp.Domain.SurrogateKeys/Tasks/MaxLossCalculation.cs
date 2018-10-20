using BankApp.Domain.SurrogateKeys.Data;

namespace BankApp.Domain.SurrogateKeys.Tasks
{
    public class MaxLossCalculation
    {
        public int MaxLossCalculationId { get; set; }

        public int DatasetId { get; set; }

        public decimal? MaxLoss { get; set; }

        public Dataset Dataset { get; set; }
    }
}
