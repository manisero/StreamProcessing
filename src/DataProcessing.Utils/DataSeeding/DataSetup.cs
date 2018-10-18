namespace DataProcessing.Utils.DataSeeding
{
    public class DataSetup
    {
        public int DatasetsCount { get; set; }

        public int ClientsPerDataset { get; set; }

        public int LoansPerClient { get; set; }

        public override string ToString()
        {
            return $"{nameof(DatasetsCount)}: {DatasetsCount}, {nameof(ClientsPerDataset)}: {ClientsPerDataset}, {nameof(LoansPerClient)}: {LoansPerClient}";
        }
    }
}
