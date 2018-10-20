namespace DataProcessing.Utils.Settings
{
    public class DataSettings
    {
        public int DatasetsCount { get; set; }

        public int ClientsPerDataset { get; set; }

        public int DepositsPerClient { get; set; }

        public int LoansPerClient { get; set; }

        public override string ToString()
        {
            return $"{nameof(DatasetsCount)}: {DatasetsCount}, {nameof(ClientsPerDataset)}: {ClientsPerDataset}, {nameof(DepositsPerClient)}: {DepositsPerClient}, {nameof(LoansPerClient)}: {LoansPerClient}";
        }
    }
}
