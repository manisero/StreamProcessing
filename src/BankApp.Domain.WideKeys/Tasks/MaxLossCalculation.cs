namespace BankApp.Domain.WideKeys.Tasks
{
    public class MaxLossCalculation
    {
        public short MaxLossCalculationId { get; set; }

        public short DatasetId { get; set; }

        public decimal? MaxLoss { get; set; }
    }
}
